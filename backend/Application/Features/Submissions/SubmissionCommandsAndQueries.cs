using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using ValidationException = Domain.Exceptions.ValidationException;
using FluentValidation;
using MediatR;

namespace Application.Features.Submissions;

public record GetSubmissionsByAssignmentQuery(Guid AssignmentId) : IRequest<IReadOnlyList<SubmissionDto>>;

public record GetMySubmissionsQuery : IRequest<IReadOnlyList<SubmissionDto>>;

public record GetSubmissionByIdQuery(Guid Id) : IRequest<SubmissionDto>;

public record CreateSubmissionCommand(CreateSubmissionDto Submission) : IRequest<SubmissionDto>;

public record UpdateSubmissionCommand(Guid Id, UpdateSubmissionDto Submission) : IRequest<SubmissionDto>;

public record GradeSubmissionCommand(Guid Id, GradeSubmissionDto Grade) : IRequest<SubmissionDto>;

public class CreateSubmissionCommandValidator : AbstractValidator<CreateSubmissionCommand>
{
    public CreateSubmissionCommandValidator()
    {
        RuleFor(x => x.Submission.AssignmentId).NotEmpty();
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Submission.AnswerText) || !string.IsNullOrWhiteSpace(x.Submission.FileUrl))
            .WithMessage("Either answer text or file URL must be provided.");
    }
}

public class UpdateSubmissionCommandValidator : AbstractValidator<UpdateSubmissionCommand>
{
    public UpdateSubmissionCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Submission.AnswerText) || !string.IsNullOrWhiteSpace(x.Submission.FileUrl))
            .WithMessage("Either answer text or file URL must be provided.");
    }
}

public class GradeSubmissionCommandValidator : AbstractValidator<GradeSubmissionCommand>
{
    public GradeSubmissionCommandValidator()
    {
        RuleFor(x => x.Grade.MarksObtained)
            .GreaterThanOrEqualTo(0).WithMessage("Marks obtained cannot be negative.");
    }
}

public class SubmissionQueryAndCommandHandler :
    IRequestHandler<GetSubmissionsByAssignmentQuery, IReadOnlyList<SubmissionDto>>,
    IRequestHandler<GetMySubmissionsQuery, IReadOnlyList<SubmissionDto>>,
    IRequestHandler<GetSubmissionByIdQuery, SubmissionDto>,
    IRequestHandler<CreateSubmissionCommand, SubmissionDto>,
    IRequestHandler<UpdateSubmissionCommand, SubmissionDto>,
    IRequestHandler<GradeSubmissionCommand, SubmissionDto>
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IStudentEnrollmentRepository _enrollmentRepository;
    private readonly ITeacherSubjectAssignmentRepository _teacherAssignmentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmissionQueryAndCommandHandler(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        IStudentEnrollmentRepository enrollmentRepository,
        ITeacherSubjectAssignmentRepository teacherAssignmentRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _enrollmentRepository = enrollmentRepository;
        _teacherAssignmentRepository = teacherAssignmentRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<SubmissionDto>> Handle(GetSubmissionsByAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), request.AssignmentId);
        }

        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        // Business Rule 4 & 7: Only assigned teacher or Admin can view submissions for an assignment
        if (role == UserRole.Teacher)
        {
            if (assignment.TeacherId != userId)
            {
                throw new ForbiddenException("You can only view submissions for assignments you created.");
            }
        }
        else if (role == UserRole.Student)
        {
            throw new ForbiddenException("Students cannot view all submissions for an assignment.");
        }

        var submissions = await _submissionRepository.GetByAssignmentIdAsync(request.AssignmentId, cancellationToken);
        return submissions.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<SubmissionDto>> Handle(GetMySubmissionsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedException();

        var submissions = await _submissionRepository.GetByStudentIdAsync(userId.Value, cancellationToken);
        return submissions.Select(MapToDto).ToList();
    }

    public async Task<SubmissionDto> Handle(GetSubmissionByIdQuery request, CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (submission == null)
        {
            throw new NotFoundException(nameof(Submission), request.Id);
        }

        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        // Role authorization check
        if (role == UserRole.Student && submission.StudentId != userId)
        {
            throw new ForbiddenException("You can only view your own submission.");
        }
        if (role == UserRole.Teacher && submission.Assignment?.TeacherId != userId)
        {
            throw new ForbiddenException("You can only view submissions for your own assignments.");
        }

        return MapToDto(submission);
    }

    public async Task<SubmissionDto> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var role = _currentUserService.Role;

        if (!userId.HasValue || role != UserRole.Student)
        {
            throw new ForbiddenException("Only students can submit answers.");
        }

        var assignment = await _assignmentRepository.GetByIdAsync(request.Submission.AssignmentId, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), request.Submission.AssignmentId);
        }

        // Business Rule 1: Cannot submit to Draft assignment
        if (assignment.Status != AssignmentStatus.Published)
        {
            throw new ValidationException("Cannot submit to an assignment that is not published.");
        }

        // Business Rule 1: Cannot submit if not enrolled in the class/course
        bool isEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(userId.Value, assignment.ClassCourseId, cancellationToken);
        if (!isEnrolled)
        {
            throw new ForbiddenException("You are not enrolled in the class for this assignment.");
        }

        // Check if student already submitted
        var existingSubmission = await _submissionRepository.GetByAssignmentAndStudentAsync(assignment.Id, userId.Value, cancellationToken);
        if (existingSubmission != null)
        {
            throw new ValidationException("You have already submitted for this assignment. Use update endpoint to edit your submission.");
        }

        // Business Rule 2: Deadline check & status determination
        var now = DateTime.UtcNow;
        SubmissionStatus initialStatus = SubmissionStatus.Submitted;

        if (now > assignment.Deadline)
        {
            if (!assignment.AllowResubmission)
            {
                throw new DeadlinePassedException();
            }
            initialStatus = SubmissionStatus.Late;
        }

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            StudentId = userId.Value,
            AnswerText = request.Submission.AnswerText,
            FileUrl = request.Submission.FileUrl,
            SubmittedAt = now,
            UpdatedAt = now,
            Status = initialStatus
        };

        await _submissionRepository.AddAsync(submission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Handle(new GetSubmissionByIdQuery(submission.Id), cancellationToken);
    }

    public async Task<SubmissionDto> Handle(UpdateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var role = _currentUserService.Role;

        var submission = await _submissionRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (submission == null)
        {
            throw new NotFoundException(nameof(Submission), request.Id);
        }

        // Business Rule 3: Student can only update their own submission
        if (submission.StudentId != userId)
        {
            throw new ForbiddenException("You can only edit your own submission.");
        }

        // Business Rule 3: Cannot edit if already graded
        if (submission.Status == SubmissionStatus.Graded)
        {
            throw new ValidationException("Cannot update submission after it has been graded.");
        }

        var assignment = submission.Assignment;
        if (assignment == null)
        {
            assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken);
        }

        // Business Rule 3: Cannot update after deadline unless resubmission allowed
        var now = DateTime.UtcNow;
        if (now > assignment!.Deadline && !assignment.AllowResubmission)
        {
            throw new DeadlinePassedException("Cannot update submission after the deadline has passed.");
        }

        submission.AnswerText = request.Submission.AnswerText;
        submission.FileUrl = request.Submission.FileUrl;
        submission.UpdatedAt = now;

        if (now > assignment.Deadline)
        {
            submission.Status = SubmissionStatus.Late;
        }
        else if (submission.Status == SubmissionStatus.ResubmissionRequired)
        {
            submission.Status = SubmissionStatus.Submitted;
        }

        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Handle(new GetSubmissionByIdQuery(submission.Id), cancellationToken);
    }

    public async Task<SubmissionDto> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var role = _currentUserService.Role;

        if (!userId.HasValue) throw new UnauthorizedException();

        var submission = await _submissionRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (submission == null)
        {
            throw new NotFoundException(nameof(Submission), request.Id);
        }

        var assignment = submission.Assignment ?? await _assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken);

        // Business Rule 7: Only assignment creator teacher or Admin can grade
        if (role == UserRole.Teacher && assignment!.TeacherId != userId)
        {
            throw new ForbiddenException("You can only grade submissions for assignments you created.");
        }

        // Business Rule 5: MarksObtained cannot exceed MaxMarks
        if (request.Grade.MarksObtained > assignment!.MaxMarks)
        {
            throw new ValidationException($"Marks obtained ({request.Grade.MarksObtained}) cannot exceed maximum marks ({assignment.MaxMarks}).");
        }

        submission.MarksObtained = request.Grade.MarksObtained;
        submission.Feedback = request.Grade.Feedback;
        submission.GradedAt = DateTime.UtcNow;
        submission.GradedByTeacherId = userId.Value;
        submission.Status = request.Grade.Status ?? SubmissionStatus.Graded;

        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Handle(new GetSubmissionByIdQuery(submission.Id), cancellationToken);
    }

    private SubmissionDto MapToDto(Submission s) => new()
    {
        Id = s.Id,
        AssignmentId = s.AssignmentId,
        AssignmentTitle = s.Assignment?.Title,
        MaxMarks = s.Assignment?.MaxMarks ?? 0,
        StudentId = s.StudentId,
        StudentName = s.Student?.Name,
        StudentEmail = s.Student?.Email,
        AnswerText = s.AnswerText,
        FileUrl = s.FileUrl,
        SubmittedAt = s.SubmittedAt,
        UpdatedAt = s.UpdatedAt,
        Status = s.Status,
        MarksObtained = s.MarksObtained,
        Feedback = s.Feedback,
        GradedAt = s.GradedAt,
        GradedByTeacherId = s.GradedByTeacherId,
        GradedByTeacherName = s.GradedByTeacher?.Name
    };
}
