using FastEndpoints;
using GameManager.Server.Services;
using Microsoft.Extensions.Options;

namespace GameManager.Server.Endpoints.Push;

public class VapidPublicKeyResponse
{
    public string PublicKey { get; set; } = string.Empty;
}

/// <summary>
/// Exposes the server's configured VAPID public key so the frontend can request push
/// subscriptions without baking the key into the deployed static assets. The public key is
/// not sensitive (it is designed to be shared with browsers), so this endpoint allows
/// anonymous access - the app needs it before a player has signed in.
/// </summary>
public class GetVapidPublicKeyEndpoint(IOptions<PushNotificationOptions> options)
    : EndpointWithoutRequest<Ok<VapidPublicKeyResponse>>
{
    public override void Configure()
    {
        Get("PublicKey");
        Group<PushGroup>();
        Version(1);
        AllowAnonymous();
    }

    public override Task<Ok<VapidPublicKeyResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var response = new VapidPublicKeyResponse { PublicKey = options.Value.VapidPublicKey };
        return Task.FromResult(TypedResults.Ok(response));
    }
}
