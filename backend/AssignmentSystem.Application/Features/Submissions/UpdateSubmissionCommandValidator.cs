using FluentValidation;

namespace AssignmentSystem.Application.Features.Submissions;

public class UpdateSubmissionCommandValidator : AbstractValidator<UpdateSubmissionCommand>
{
    public UpdateSubmissionCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Submission.AnswerText) || !string.IsNullOrWhiteSpace(x.Submission.FileUrl))
            .WithMessage("Either answer text or file URL must be provided.");
    }
}
