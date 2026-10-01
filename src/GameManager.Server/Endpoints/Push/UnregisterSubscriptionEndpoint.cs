using FastEndpoints;
using GameManager.Application.Features.Push.Commands.UnregisterSubscription;
using FluentValidation;

namespace GameManager.Server.Endpoints.Push;

public class UnregisterSubscriptionDTO
{
    public string Endpoint { get; set; } = string.Empty;
}

public class UnregisterSubscriptionDTOValidator : Validator<UnregisterSubscriptionDTO>
{
    public UnregisterSubscriptionDTOValidator()
    {
        RuleFor(request => request.Endpoint)
            .NotEmpty()
            .MaximumLength(8192)
            .Must(endpoint => Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) &&
                              uri.Scheme == Uri.UriSchemeHttps)
            .WithMessage("Endpoint must be an absolute HTTPS URL.");
    }
}

public class UnregisterSubscriptionEndpoint(IMediator mediator)
    : Endpoint<UnregisterSubscriptionDTO, Results<Ok, ProblemDetails>>
{
    public override void Configure()
    {
        Post("Unsubscribe");
        Group<PushGroup>();
        Version(1);
    }

    public override async Task<Results<Ok, ProblemDetails>> ExecuteAsync(
        UnregisterSubscriptionDTO request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UnregisterPushSubscriptionCommand(request.Endpoint),
            cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : result.Error.ToProblemDetails();
    }
}
