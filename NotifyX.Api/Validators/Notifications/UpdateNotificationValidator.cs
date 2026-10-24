
using FluentValidation;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Validators;

public sealed class UpdateNotificationValidator
    : AbstractValidator<UpdateNotificationRequest>
{
    public UpdateNotificationValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.")
            .When(x => x.Title is not null);

        RuleFor(x => x.Body)
            .MaximumLength(5000)
            .WithMessage("Body cannot exceed 5000 characters.")
            .When(x => x.Body is not null);

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid notification type.")
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 5)
            .WithMessage("Priority must be between 1 and 5.")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.ScheduledAt)
            .Must(date => date >= DateTimeOffset.UtcNow)
            .WithMessage("ScheduledAt must be in the future.")
            .When(x => x.ScheduledAt.HasValue);

        RuleFor(x => x.Metadata)
            .MaximumLength(10000)
            .WithMessage("Metadata cannot exceed 10000 characters.")
            .When(x => x.Metadata is not null);
    }
}