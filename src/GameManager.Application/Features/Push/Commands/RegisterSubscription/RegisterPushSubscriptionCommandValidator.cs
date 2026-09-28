using System.Net;

namespace GameManager.Application.Features.Push.Commands.RegisterSubscription;

public class RegisterPushSubscriptionCommandValidator : AbstractValidator<RegisterPushSubscriptionCommand>
{
    public RegisterPushSubscriptionCommandValidator()
    {
        RuleFor(command => command.PlayerId).NotEmpty();
        RuleFor(command => command.GameId).NotEmpty();
        RuleFor(command => command.Endpoint)
            .NotEmpty()
            .MaximumLength(8192)
            .Must(endpoint => Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) &&
                              uri.Scheme == Uri.UriSchemeHttps)
            .WithMessage("Endpoint must be an absolute HTTPS URL.");
        RuleFor(command => command.P256dh).NotEmpty().MaximumLength(256);
        RuleFor(command => command.Auth).NotEmpty().MaximumLength(256);
    }
}
