namespace GameManager.Application.Features.Push.Commands.UnregisterSubscription;

public class UnregisterPushSubscriptionCommandValidator : AbstractValidator<UnregisterPushSubscriptionCommand>
{
    public UnregisterPushSubscriptionCommandValidator()
    {
        RuleFor(command => command.Endpoint)
            .NotEmpty()
            .MaximumLength(8192)
            .Must(endpoint => Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) &&
                              uri.Scheme == Uri.UriSchemeHttps)
            .WithMessage("Endpoint must be an absolute HTTPS URL.");
    }
}
