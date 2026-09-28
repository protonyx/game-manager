using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Push.Commands.RegisterSubscription;

public record RegisterPushSubscriptionCommand(
    Guid PlayerId,
    Guid GameId,
    string Endpoint,
    string P256dh,
    string Auth) : ICommand;
