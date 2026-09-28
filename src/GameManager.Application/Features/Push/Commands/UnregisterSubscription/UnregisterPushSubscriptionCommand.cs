using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Push.Commands.UnregisterSubscription;

public record UnregisterPushSubscriptionCommand(string Endpoint) : ICommand;
