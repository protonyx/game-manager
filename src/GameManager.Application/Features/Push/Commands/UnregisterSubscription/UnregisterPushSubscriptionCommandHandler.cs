using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Errors;

namespace GameManager.Application.Features.Push.Commands.UnregisterSubscription;

public class UnregisterPushSubscriptionCommandHandler(IPushSubscriptionRepository subscriptions)
    : ICommandHandler<UnregisterPushSubscriptionCommand>
{
    public async Task<UnitResult<ApplicationError>> Handle(
        UnregisterPushSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        await subscriptions.DeleteByEndpointAsync(request.Endpoint, cancellationToken);
        return UnitResult.Success<ApplicationError>();
    }
}
