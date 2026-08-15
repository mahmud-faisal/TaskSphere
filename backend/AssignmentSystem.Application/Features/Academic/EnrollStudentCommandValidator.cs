using FluentValidation;

namespace AssignmentSystem.Application.Features.Academic;

public class EnrollStudentCommandValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentCommandValidator()
    {
        RuleFor(x => x.Enrollment.StudentId).NotEmpty();
        RuleFor(x => x.Enrollment.ClassCourseId).NotEmpty();
    }
}
