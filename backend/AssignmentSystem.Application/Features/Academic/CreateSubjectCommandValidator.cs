using FluentValidation;

namespace AssignmentSystem.Application.Features.Academic;

public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(x => x.Subject.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Subject.ClassCourseId).NotEmpty();
    }
}
