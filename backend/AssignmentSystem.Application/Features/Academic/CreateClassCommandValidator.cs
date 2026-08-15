using FluentValidation;

namespace AssignmentSystem.Application.Features.Academic;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.ClassCourse.Name).NotEmpty().MaximumLength(100);
    }
}
