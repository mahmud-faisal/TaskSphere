using Application.Common.Interfaces;
using Application.DTOs;
using Application.Features.Assignments;
using Application.Features.Submissions;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests;

public class BusinessRuleTests
{
    private readonly Mock<IAssignmentRepository> _mockAssignmentRepo = new();
    private readonly Mock<ISubmissionRepository> _mockSubmissionRepo = new();
    private readonly Mock<IStudentEnrollmentRepository> _mockEnrollmentRepo = new();
    private readonly Mock<ITeacherSubjectAssignmentRepository> _mockTeacherAssignmentRepo = new();
    private readonly Mock<ICurrentUserService> _mockCurrentUserService = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();

    private readonly SubmissionQueryAndCommandHandler _submissionHandler;
    private readonly AssignmentQueryAndCommandHandler _assignmentHandler;

    public BusinessRuleTests()
    {
        _submissionHandler = new SubmissionQueryAndCommandHandler(
            _mockSubmissionRepo.Object,
            _mockAssignmentRepo.Object,
            _mockEnrollmentRepo.Object,
            _mockTeacherAssignmentRepo.Object,
            _mockCurrentUserService.Object,
            _mockUnitOfWork.Object);

        Mock<ISubjectRepository> mockSubjectRepo = new();
        Mock<IClassCourseRepository> mockClassRepo = new();
        Mock<IUserRepository> mockUserRepo = new();

        _assignmentHandler = new AssignmentQueryAndCommandHandler(
            _mockAssignmentRepo.Object,
            _mockTeacherAssignmentRepo.Object,
            _mockEnrollmentRepo.Object,
            mockSubjectRepo.Object,
            mockClassRepo.Object,
            mockUserRepo.Object,
            _mockCurrentUserService.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task StudentCannotSubmitToDraftAssignment_ThrowsValidationException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(studentId);
        _mockCurrentUserService.Setup(s => s.Role).Returns(UserRole.Student);

        _mockAssignmentRepo.Setup(r => r.GetByIdAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Assignment
            {
                Id = assignmentId,
                Status = AssignmentStatus.Draft, // Draft assignment
                ClassCourseId = classId,
                Deadline = DateTime.UtcNow.AddDays(1)
            });

        _mockEnrollmentRepo.Setup(r => r.IsStudentEnrolledAsync(studentId, classId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateSubmissionCommand(new CreateSubmissionDto
        {
            AssignmentId = assignmentId,
            AnswerText = "My Draft Answer"
        });

        // Act
        Func<Task> act = async () => await _submissionHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Domain.Exceptions.ValidationException>()
            .WithMessage("*not published*");
    }

    [Fact]
    public async Task StudentCannotSubmitToUnenrolledClassAssignment_ThrowsForbiddenException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(studentId);
        _mockCurrentUserService.Setup(s => s.Role).Returns(UserRole.Student);

        _mockAssignmentRepo.Setup(r => r.GetByIdAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Assignment
            {
                Id = assignmentId,
                Status = AssignmentStatus.Published,
                ClassCourseId = classId,
                Deadline = DateTime.UtcNow.AddDays(1)
            });

        _mockEnrollmentRepo.Setup(r => r.IsStudentEnrolledAsync(studentId, classId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Student not enrolled

        var command = new CreateSubmissionCommand(new CreateSubmissionDto
        {
            AssignmentId = assignmentId,
            AnswerText = "Test Solution"
        });

        // Act
        Func<Task> act = async () => await _submissionHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*not enrolled*");
    }

    [Fact]
    public async Task StudentCannotSubmitAfterDeadlineWhenNotAllowed_ThrowsDeadlinePassedException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(studentId);
        _mockCurrentUserService.Setup(s => s.Role).Returns(UserRole.Student);

        _mockAssignmentRepo.Setup(r => r.GetByIdAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Assignment
            {
                Id = assignmentId,
                Status = AssignmentStatus.Published,
                ClassCourseId = classId,
                Deadline = DateTime.UtcNow.AddDays(-1), // Past deadline
                AllowResubmission = false
            });

        _mockEnrollmentRepo.Setup(r => r.IsStudentEnrolledAsync(studentId, classId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockSubmissionRepo.Setup(r => r.GetByAssignmentAndStudentAsync(assignmentId, studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Submission?)null);

        var command = new CreateSubmissionCommand(new CreateSubmissionDto
        {
            AssignmentId = assignmentId,
            AnswerText = "Late Answer"
        });

        // Act
        Func<Task> act = async () => await _submissionHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DeadlinePassedException>();
    }

    [Fact]
    public async Task TeacherCannotGradeExceedingMaxMarks_ThrowsValidationException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(teacherId);
        _mockCurrentUserService.Setup(s => s.Role).Returns(UserRole.Teacher);

        var assignment = new Assignment { Id = assignmentId, TeacherId = teacherId, MaxMarks = 100 };
        var submission = new Submission { Id = submissionId, AssignmentId = assignmentId, Assignment = assignment };

        _mockSubmissionRepo.Setup(r => r.GetWithDetailsAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var command = new GradeSubmissionCommand(submissionId, new GradeSubmissionDto
        {
            MarksObtained = 150, // Exceeds MaxMarks of 100
            Feedback = "Over-allocated marks test"
        });

        // Act
        Func<Task> act = async () => await _submissionHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Domain.Exceptions.ValidationException>()
            .WithMessage("*cannot exceed maximum marks*");
    }

    [Fact]
    public async Task TeacherCannotCreateAssignmentForUnassignedSubject_ThrowsForbiddenException()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(teacherId);
        _mockCurrentUserService.Setup(s => s.Role).Returns(UserRole.Teacher);

        _mockTeacherAssignmentRepo.Setup(r => r.IsTeacherAssignedAsync(teacherId, subjectId, classId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Teacher is not assigned to this subject/class

        var command = new CreateAssignmentCommand(new CreateAssignmentDto
        {
            Title = "Unassigned Subject Assignment",
            SubjectId = subjectId,
            ClassCourseId = classId,
            Deadline = DateTime.UtcNow.AddDays(7),
            MaxMarks = 100
        });

        // Act
        Func<Task> act = async () => await _assignmentHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*not assigned to teach*");
    }

    [Fact]
    public async Task StudentCannotUpdateGradedSubmission_ThrowsValidationException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        _mockCurrentUserService.Setup(s => s.UserId).Returns(studentId);
        _mockCurrentUserService.Setup(s => s.Role).Returns(UserRole.Student);

        var assignment = new Assignment { Id = assignmentId, Deadline = DateTime.UtcNow.AddDays(5) };
        var submission = new Submission
        {
            Id = submissionId,
            AssignmentId = assignmentId,
            StudentId = studentId,
            Status = SubmissionStatus.Graded, // Already graded
            Assignment = assignment
        };

        _mockSubmissionRepo.Setup(r => r.GetWithDetailsAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var command = new UpdateSubmissionCommand(submissionId, new UpdateSubmissionDto
        {
            AnswerText = "Attempting to change graded submission"
        });

        // Act
        Func<Task> act = async () => await _submissionHandler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Domain.Exceptions.ValidationException>()
            .WithMessage("*after it has been graded*");
    }
}
