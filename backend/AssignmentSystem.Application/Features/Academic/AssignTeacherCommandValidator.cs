using FluentValidation;

namespace AssignmentSystem.Application.Features.Academic;

public class AssignTeacherCommandValidator : AbstractValidator<AssignTeacherCommand>
{
    public AssignTeacherCommandValidator()
    {
        RuleFor(x => x.Assignment.TeacherId).NotEmpty();
        RuleFor(x => x.Assignment.SubjectId).NotEmpty();
        RuleFor(x => x.Assignment.ClassCourseId).NotEmpty();
    }
}
