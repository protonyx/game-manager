using FastEndpoints;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Push.Commands.RegisterSubscription;
using FluentValidation;

namespace GameManager.Server.Endpoints.Push;

public class RegisterSubscriptionDTO
{
    public string Endpoint { get; set; } = string.Empty;
    public string P256dh { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
}

public class RegisterSubscriptionDTOValidator : Validator<RegisterSubscriptionDTO>
{
    public RegisterSubscriptionDTOValidator()
    {
        RuleFor(request => request.Endpoint)
            .NotEmpty()
            .MaximumLength(8192)
            .Must(endpoint => Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) &&
                              uri.Scheme == Uri.UriSchemeHttps)
            .WithMessage("Endpoint must be an absolute HTTPS URL.");
        RuleFor(request => request.P256dh).NotEmpty().MaximumLength(256);
        RuleFor(request => request.Auth).NotEmpty().MaximumLength(256);
    }
}

public class RegisterSubscriptionEndpoint : Endpoint<RegisterSubscriptionDTO, Results<Ok, ProblemDetails>>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;

    public RegisterSubscriptionEndpoint(IMediator mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Post("Subscribe");
        Group<PushGroup>();
        Version(1);
    }

    public override async Task<Results<Ok, ProblemDetails>> ExecuteAsync(
        RegisterSubscriptionDTO request,
        CancellationToken cancellationToken)
    {
        var playerId = _userContext.User?.GetPlayerId();
        var gameId = _userContext.User?.GetGameId();
        if (playerId is null || gameId is null)
        {
            return ApplicationError.Authorization("Not authorized").ToProblemDetails();
        }

        var result = await _mediator.Send(
            new RegisterPushSubscriptionCommand(
                playerId.Value, gameId.Value, request.Endpoint, request.P256dh, request.Auth),
            cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : result.Error.ToProblemDetails();
    }
}
