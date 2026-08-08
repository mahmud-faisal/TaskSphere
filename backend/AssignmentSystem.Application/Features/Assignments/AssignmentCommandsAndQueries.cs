using AssignmentSystem.Application.Common.Interfaces;
using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Domain.Enums;
using AssignmentSystem.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace AssignmentSystem.Application.Features.Assignments;

public record GetAssignmentsQuery(Guid? ClassCourseId = null, Guid? SubjectId = null) : IRequest<IReadOnlyList<AssignmentDto>>;

public record GetAssignmentByIdQuery(Guid Id) : IRequest<AssignmentDto>;

public record CreateAssignmentCommand(CreateAssignmentDto Assignment) : IRequest<AssignmentDto>;

public record UpdateAssignmentCommand(Guid Id, UpdateAssignmentDto Assignment) : IRequest<AssignmentDto>;

public record PublishAssignmentCommand(Guid Id) : IRequest<AssignmentDto>;

public record DeleteAssignmentCommand(Guid Id) : IRequest<bool>;

public class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(x => x.Assignment.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Assignment.SubjectId).NotEmpty();
        RuleFor(x => x.Assignment.ClassCourseId).NotEmpty();
        RuleFor(x => x.Assignment.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
        RuleFor(x => x.Assignment.MaxMarks).GreaterThan(0).WithMessage("MaxMarks must be greater than 0.");
    }
}

public class UpdateAssignmentCommandValidator : AbstractValidator<UpdateAssignmentCommand>
{
    public UpdateAssignmentCommandValidator()
    {
        RuleFor(x => x.Assignment.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Assignment.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
        RuleFor(x => x.Assignment.MaxMarks).GreaterThan(0).WithMessage("MaxMarks must be greater than 0.");
    }
}

public class AssignmentQueryAndCommandHandler :
    IRequestHandler<GetAssignmentsQuery, IReadOnlyList<AssignmentDto>>,
    IRequestHandler<GetAssignmentByIdQuery, AssignmentDto>,
    IRequestHandler<CreateAssignmentCommand, AssignmentDto>,
    IRequestHandler<UpdateAssignmentCommand, AssignmentDto>,
    IRequestHandler<PublishAssignmentCommand, AssignmentDto>,
    IRequestHandler<DeleteAssignmentCommand, bool>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ITeacherSubjectAssignmentRepository _teacherAssignmentRepository;
    private readonly IStudentEnrollmentRepository _enrollmentRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IClassCourseRepository _classRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignmentQueryAndCommandHandler(
        IAssignmentRepository assignmentRepository,
        ITeacherSubjectAssignmentRepository teacherAssignmentRepository,
        IStudentEnrollmentRepository enrollmentRepository,
        ISubjectRepository subjectRepository,
        IClassCourseRepository classRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _teacherAssignmentRepository = teacherAssignmentRepository;
        _enrollmentRepository = enrollmentRepository;
        _subjectRepository = subjectRepository;
        _classRepository = classRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        IReadOnlyList<Assignment> assignments;

        if (role == UserRole.Student)
        {
            if (!userId.HasValue) throw new UnauthorizedException();
            
            // Get classes student is enrolled in
            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(userId.Value, cancellationToken);
            var enrolledClassIds = enrollments.Select(e => e.ClassCourseId).ToHashSet();

            var all = await _assignmentRepository.GetAllAsync(cancellationToken);
            // Business Rule 1 & 8: Student sees only Published assignments for their enrolled class
            assignments = all.Where(a => a.Status == AssignmentStatus.Published && enrolledClassIds.Contains(a.ClassCourseId)).ToList();
        }
        else if (role == UserRole.Teacher)
        {
            if (!userId.HasValue) throw new UnauthorizedException();
            
            // Teacher sees assignments created by them or for their subjects
            var all = await _assignmentRepository.GetAllAsync(cancellationToken);
            assignments = all.Where(a => a.TeacherId == userId.Value).ToList();
        }
        else // Admin or anonymous
        {
            assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
        }

        if (request.ClassCourseId.HasValue)
        {
            assignments = assignments.Where(a => a.ClassCourseId == request.ClassCourseId.Value).ToList();
        }
        if (request.SubjectId.HasValue)
        {
            assignments = assignments.Where(a => a.SubjectId == request.SubjectId.Value).ToList();
        }

        return MapToDtos(assignments);
    }

    public async Task<AssignmentDto> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), request.Id);
        }

        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        if (role == UserRole.Student)
        {
            // Business Rule 1: A student cannot view/submit to a Draft assignment
            if (assignment.Status == AssignmentStatus.Draft)
            {
                throw new ForbiddenException("Draft assignments are not accessible to students.");
            }

            // Check student enrollment
            if (userId.HasValue)
            {
                bool enrolled = await _enrollmentRepository.IsStudentEnrolledAsync(userId.Value, assignment.ClassCourseId, cancellationToken);
                if (!enrolled)
                {
                    throw new ForbiddenException("You are not enrolled in the class for this assignment.");
                }
            }
        }

        return MapToDto(assignment);
    }

    public async Task<AssignmentDto> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        if (!userId.HasValue) throw new UnauthorizedException();

        // Business Rule 4: Teacher can only manage assignments for subjects/classes they are assigned to teach
        if (role == UserRole.Teacher)
        {
            bool isAssigned = await _teacherAssignmentRepository.IsTeacherAssignedAsync(
                userId.Value, request.Assignment.SubjectId, request.Assignment.ClassCourseId, cancellationToken);

            if (!isAssigned)
            {
                throw new ForbiddenException("You are not assigned to teach this subject in this class.");
            }
        }

        var entity = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = request.Assignment.Title,
            Description = request.Assignment.Description,
            SubjectId = request.Assignment.SubjectId,
            ClassCourseId = request.Assignment.ClassCourseId,
            TeacherId = userId.Value,
            Deadline = request.Assignment.Deadline.ToUniversalTime(),
            MaxMarks = request.Assignment.MaxMarks,
            Status = request.Assignment.PublishImmediately ? AssignmentStatus.Published : AssignmentStatus.Draft,
            AllowResubmission = request.Assignment.AllowResubmission,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _assignmentRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Handle(new GetAssignmentByIdQuery(entity.Id), cancellationToken);
    }

    public async Task<AssignmentDto> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), request.Id);
        }

        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        if (role == UserRole.Teacher && assignment.TeacherId != userId)
        {
            throw new ForbiddenException("You can only edit assignments you created.");
        }

        assignment.Title = request.Assignment.Title;
        assignment.Description = request.Assignment.Description;
        assignment.Deadline = request.Assignment.Deadline.ToUniversalTime();
        assignment.MaxMarks = request.Assignment.MaxMarks;
        assignment.AllowResubmission = request.Assignment.AllowResubmission;
        assignment.UpdatedAt = DateTime.UtcNow;

        _assignmentRepository.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Handle(new GetAssignmentByIdQuery(assignment.Id), cancellationToken);
    }

    public async Task<AssignmentDto> Handle(PublishAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), request.Id);
        }

        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        if (role == UserRole.Teacher && assignment.TeacherId != userId)
        {
            throw new ForbiddenException("You can only publish assignments you created.");
        }

        assignment.Status = AssignmentStatus.Published;
        assignment.UpdatedAt = DateTime.UtcNow;

        _assignmentRepository.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Handle(new GetAssignmentByIdQuery(assignment.Id), cancellationToken);
    }

    public async Task<bool> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), request.Id);
        }

        var role = _currentUserService.Role;
        var userId = _currentUserService.UserId;

        if (role == UserRole.Teacher && assignment.TeacherId != userId)
        {
            throw new ForbiddenException("You can only delete assignments you created.");
        }

        _assignmentRepository.Delete(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private AssignmentDto MapToDto(Assignment a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Description = a.Description,
        SubjectId = a.SubjectId,
        SubjectName = a.Subject?.Name,
        ClassCourseId = a.ClassCourseId,
        ClassCourseName = a.ClassCourse?.Name,
        TeacherId = a.TeacherId,
        TeacherName = a.Teacher?.Name,
        Deadline = a.Deadline,
        MaxMarks = a.MaxMarks,
        Status = a.Status,
        AllowResubmission = a.AllowResubmission,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt,
        SubmissionsCount = a.Submissions?.Count ?? 0
    };

    private IReadOnlyList<AssignmentDto> MapToDtos(IEnumerable<Assignment> list)
    {
        return list.Select(MapToDto).ToList();
    }
}
