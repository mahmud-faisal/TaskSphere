using FluentValidation;

namespace AssignmentSystem.Application.Features.Assignments;

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
