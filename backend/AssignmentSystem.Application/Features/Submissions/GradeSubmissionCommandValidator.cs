using FluentValidation;

namespace AssignmentSystem.Application.Features.Submissions;

public class GradeSubmissionCommandValidator : AbstractValidator<GradeSubmissionCommand>
{
    public GradeSubmissionCommandValidator()
    {
        RuleFor(x => x.Grade.MarksObtained)
            .GreaterThanOrEqualTo(0).WithMessage("Marks obtained cannot be negative.");
    }
}
