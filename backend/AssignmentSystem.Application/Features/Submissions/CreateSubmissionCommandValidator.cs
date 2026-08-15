using FluentValidation;

namespace AssignmentSystem.Application.Features.Submissions;

public class CreateSubmissionCommandValidator : AbstractValidator<CreateSubmissionCommand>
{
    public CreateSubmissionCommandValidator()
    {
        RuleFor(x => x.Submission.AssignmentId).NotEmpty();
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Submission.AnswerText) || !string.IsNullOrWhiteSpace(x.Submission.FileUrl))
            .WithMessage("Either answer text or file URL must be provided.");
    }
}
