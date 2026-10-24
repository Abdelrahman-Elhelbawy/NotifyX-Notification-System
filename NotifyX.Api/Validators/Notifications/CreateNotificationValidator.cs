using FluentValidation;
using NotifyX.Api.Models.Notifications;

namespace NotifyX.Api.Validators;

public sealed class CreateNotificationValidator
    : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationValidator()
    {
        RuleFor(x => x.UserIds)
            .NotNull()
            .WithMessage("Recipients are required.")
            .NotEmpty()
            .WithMessage("At least one recipient is required.");

        RuleForEach(x => x.UserIds)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MaximumLength(100)
            .WithMessage("UserId cannot exceed 100 characters.");

        RuleFor(x => x.UserIds)
            .Must(ids => ids == null ||
                        ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate recipients are not allowed.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Body is required.")
            .MaximumLength(5000)
            .WithMessage("Body cannot exceed 5000 characters.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid notification type.");

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 5)
            .WithMessage("Priority must be between 1 and 5.");

        RuleFor(x => x.Metadata)
            .MaximumLength(10000)
            .WithMessage("Metadata cannot exceed 10000 characters.");
    }
}
