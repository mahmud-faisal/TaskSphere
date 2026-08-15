using FluentValidation;

namespace AssignmentSystem.Application.Features.Assignments;

public class UpdateAssignmentCommandValidator : AbstractValidator<UpdateAssignmentCommand>
{
    public UpdateAssignmentCommandValidator()
    {
        RuleFor(x => x.Assignment.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Assignment.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
        RuleFor(x => x.Assignment.MaxMarks).GreaterThan(0).WithMessage("MaxMarks must be greater than 0.");
    }
}
