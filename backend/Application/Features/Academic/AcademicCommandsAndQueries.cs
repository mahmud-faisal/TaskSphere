using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using ValidationException = Domain.Exceptions.ValidationException;
using FluentValidation;
using MediatR;

namespace Application.Features.Academic;

// Queries & Commands
public record GetClassesQuery : IRequest<IReadOnlyList<ClassCourseDto>>;
public record CreateClassCommand(CreateClassCourseDto ClassCourse) : IRequest<ClassCourseDto>;

public record GetSubjectsQuery(Guid? ClassCourseId = null) : IRequest<IReadOnlyList<SubjectDto>>;
public record CreateSubjectCommand(CreateSubjectDto Subject) : IRequest<SubjectDto>;

public record GetTeacherAssignmentsQuery(Guid? TeacherId = null) : IRequest<IReadOnlyList<TeacherSubjectAssignmentDto>>;
public record AssignTeacherCommand(AssignTeacherDto Assignment) : IRequest<TeacherSubjectAssignmentDto>;

public record GetStudentEnrollmentsQuery(Guid? StudentId = null, Guid? ClassCourseId = null) : IRequest<IReadOnlyList<StudentEnrollmentDto>>;
public record EnrollStudentCommand(EnrollStudentDto Enrollment) : IRequest<StudentEnrollmentDto>;

// Validators
public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.ClassCourse.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(x => x.Subject.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Subject.ClassCourseId).NotEmpty();
    }
}

public class AssignTeacherCommandValidator : AbstractValidator<AssignTeacherCommand>
{
    public AssignTeacherCommandValidator()
    {
        RuleFor(x => x.Assignment.TeacherId).NotEmpty();
        RuleFor(x => x.Assignment.SubjectId).NotEmpty();
        RuleFor(x => x.Assignment.ClassCourseId).NotEmpty();
    }
}

public class EnrollStudentCommandValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentCommandValidator()
    {
        RuleFor(x => x.Enrollment.StudentId).NotEmpty();
        RuleFor(x => x.Enrollment.ClassCourseId).NotEmpty();
    }
}

// Handler
public class AcademicQueryAndCommandHandler :
    IRequestHandler<GetClassesQuery, IReadOnlyList<ClassCourseDto>>,
    IRequestHandler<CreateClassCommand, ClassCourseDto>,
    IRequestHandler<GetSubjectsQuery, IReadOnlyList<SubjectDto>>,
    IRequestHandler<CreateSubjectCommand, SubjectDto>,
    IRequestHandler<GetTeacherAssignmentsQuery, IReadOnlyList<TeacherSubjectAssignmentDto>>,
    IRequestHandler<AssignTeacherCommand, TeacherSubjectAssignmentDto>,
    IRequestHandler<GetStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>,
    IRequestHandler<EnrollStudentCommand, StudentEnrollmentDto>
{
    private readonly IClassCourseRepository _classRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ITeacherSubjectAssignmentRepository _teacherAssignmentRepository;
    private readonly IStudentEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcademicQueryAndCommandHandler(
        IClassCourseRepository classRepository,
        ISubjectRepository subjectRepository,
        ITeacherSubjectAssignmentRepository teacherAssignmentRepository,
        IStudentEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _classRepository = classRepository;
        _subjectRepository = subjectRepository;
        _teacherAssignmentRepository = teacherAssignmentRepository;
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ClassCourseDto>> Handle(GetClassesQuery request, CancellationToken cancellationToken)
    {
        var classes = await _classRepository.GetAllAsync(cancellationToken);
        return classes.Select(c => new ClassCourseDto { Id = c.Id, Name = c.Name, Description = c.Description }).ToList();
    }

    public async Task<ClassCourseDto> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var entity = new ClassCourse
        {
            Id = Guid.NewGuid(),
            Name = request.ClassCourse.Name,
            Description = request.ClassCourse.Description
        };

        await _classRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClassCourseDto { Id = entity.Id, Name = entity.Name, Description = entity.Description };
    }

    public async Task<IReadOnlyList<SubjectDto>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Subject> subjects;
        if (request.ClassCourseId.HasValue)
        {
            subjects = await _subjectRepository.GetByClassCourseIdAsync(request.ClassCourseId.Value, cancellationToken);
        }
        else
        {
            subjects = await _subjectRepository.GetAllAsync(cancellationToken);
        }

        var classes = (await _classRepository.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id, c => c.Name);

        return subjects.Select(s => new SubjectDto
        {
            Id = s.Id,
            Name = s.Name,
            ClassCourseId = s.ClassCourseId,
            ClassCourseName = classes.GetValueOrDefault(s.ClassCourseId)
        }).ToList();
    }

    public async Task<SubjectDto> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var classCourse = await _classRepository.GetByIdAsync(request.Subject.ClassCourseId, cancellationToken);
        if (classCourse == null)
        {
            throw new NotFoundException(nameof(ClassCourse), request.Subject.ClassCourseId);
        }

        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = request.Subject.Name,
            ClassCourseId = request.Subject.ClassCourseId
        };

        await _subjectRepository.AddAsync(subject, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            ClassCourseId = subject.ClassCourseId,
            ClassCourseName = classCourse.Name
        };
    }

    public async Task<IReadOnlyList<TeacherSubjectAssignmentDto>> Handle(GetTeacherAssignmentsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<TeacherSubjectAssignment> assignments;
        if (request.TeacherId.HasValue)
        {
            assignments = await _teacherAssignmentRepository.GetByTeacherIdAsync(request.TeacherId.Value, cancellationToken);
        }
        else
        {
            assignments = await _teacherAssignmentRepository.GetAllAsync(cancellationToken);
        }

        var users = (await _userRepository.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id, u => u.Name);
        var subjects = (await _subjectRepository.GetAllAsync(cancellationToken)).ToDictionary(s => s.Id, s => s.Name);
        var classes = (await _classRepository.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id, c => c.Name);

        return assignments.Select(a => new TeacherSubjectAssignmentDto
        {
            Id = a.Id,
            TeacherId = a.TeacherId,
            TeacherName = users.GetValueOrDefault(a.TeacherId),
            SubjectId = a.SubjectId,
            SubjectName = subjects.GetValueOrDefault(a.SubjectId),
            ClassCourseId = a.ClassCourseId,
            ClassCourseName = classes.GetValueOrDefault(a.ClassCourseId)
        }).ToList();
    }

    public async Task<TeacherSubjectAssignmentDto> Handle(AssignTeacherCommand request, CancellationToken cancellationToken)
    {
        bool alreadyAssigned = await _teacherAssignmentRepository.IsTeacherAssignedAsync(
            request.Assignment.TeacherId, request.Assignment.SubjectId, request.Assignment.ClassCourseId, cancellationToken);

        if (alreadyAssigned)
        {
            throw new ValidationException("Teacher is already assigned to this subject and class.");
        }

        var assignment = new TeacherSubjectAssignment
        {
            Id = Guid.NewGuid(),
            TeacherId = request.Assignment.TeacherId,
            SubjectId = request.Assignment.SubjectId,
            ClassCourseId = request.Assignment.ClassCourseId
        };

        await _teacherAssignmentRepository.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var teacher = await _userRepository.GetByIdAsync(assignment.TeacherId, cancellationToken);
        var subject = await _subjectRepository.GetByIdAsync(assignment.SubjectId, cancellationToken);
        var classCourse = await _classRepository.GetByIdAsync(assignment.ClassCourseId, cancellationToken);

        return new TeacherSubjectAssignmentDto
        {
            Id = assignment.Id,
            TeacherId = assignment.TeacherId,
            TeacherName = teacher?.Name,
            SubjectId = assignment.SubjectId,
            SubjectName = subject?.Name,
            ClassCourseId = assignment.ClassCourseId,
            ClassCourseName = classCourse?.Name
        };
    }

    public async Task<IReadOnlyList<StudentEnrollmentDto>> Handle(GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<StudentEnrollment> enrollments;
        if (request.StudentId.HasValue)
        {
            enrollments = await _enrollmentRepository.GetByStudentIdAsync(request.StudentId.Value, cancellationToken);
        }
        else
        {
            enrollments = await _enrollmentRepository.GetAllAsync(cancellationToken);
        }

        if (request.ClassCourseId.HasValue)
        {
            enrollments = enrollments.Where(e => e.ClassCourseId == request.ClassCourseId.Value).ToList();
        }

        var users = (await _userRepository.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id, u => u);
        var classes = (await _classRepository.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id, c => c.Name);

        return enrollments.Select(e => new StudentEnrollmentDto
        {
            Id = e.Id,
            StudentId = e.StudentId,
            StudentName = users.TryGetValue(e.StudentId, out var u) ? u.Name : null,
            StudentEmail = u?.Email,
            ClassCourseId = e.ClassCourseId,
            ClassCourseName = classes.GetValueOrDefault(e.ClassCourseId)
        }).ToList();
    }

    public async Task<StudentEnrollmentDto> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        bool alreadyEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(
            request.Enrollment.StudentId, request.Enrollment.ClassCourseId, cancellationToken);

        if (alreadyEnrolled)
        {
            throw new ValidationException("Student is already enrolled in this class.");
        }

        var enrollment = new StudentEnrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.Enrollment.StudentId,
            ClassCourseId = request.Enrollment.ClassCourseId
        };

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var student = await _userRepository.GetByIdAsync(enrollment.StudentId, cancellationToken);
        var classCourse = await _classRepository.GetByIdAsync(enrollment.ClassCourseId, cancellationToken);

        return new StudentEnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentName = student?.Name,
            StudentEmail = student?.Email,
            ClassCourseId = enrollment.ClassCourseId,
            ClassCourseName = classCourse?.Name
        };
    }
}
