
using FluentValidation;
using NotifyX.Api.Models.Notifications;

public sealed class CreateNotificationValidator
    : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationValidator()
    {
        RuleFor(x => x.UserIds)
            .NotEmpty()
            .WithMessage("At least one recipient is required.")
            .Must(ids => ids is not null &&
                        ids.Any(id => !string.IsNullOrWhiteSpace(id)))
            .WithMessage("At least one valid recipient is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Body is required.");
    }
}
