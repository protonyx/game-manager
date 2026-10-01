using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;

namespace GameManager.Application.Features.Push.Commands.RegisterSubscription;

public class RegisterPushSubscriptionCommandHandler(IPushSubscriptionRepository subscriptions)
    : ICommandHandler<RegisterPushSubscriptionCommand>
{
    public async Task<UnitResult<ApplicationError>> Handle(
        RegisterPushSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        if (await subscriptions.GetByEndpointAsync(request.Endpoint, cancellationToken) is not null)
        {
            await subscriptions.DeleteByEndpointAsync(request.Endpoint, cancellationToken);
        }

        await subscriptions.CreateAsync(
            new PushSubscription(request.PlayerId, request.GameId, request.Endpoint, request.P256dh, request.Auth),
            cancellationToken);

        return UnitResult.Success<ApplicationError>();
    }
}
