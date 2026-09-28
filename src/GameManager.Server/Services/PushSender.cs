using System.Net;
using System.Text.Json;
using GameManager.Application.Contracts;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Options;
using WebPushSubscription = Lib.Net.Http.WebPush.PushSubscription;

namespace GameManager.Server.Services;

public class PushSender : IPushSender
{
    private readonly PushServiceClient _client;
    private readonly ILogger<PushSender> _logger;

    public PushSender(
        PushServiceClient client,
        IOptions<PushNotificationOptions> options,
        ILogger<PushSender> logger)
    {
        _client = client;
        _logger = logger;
        var configuration = options.Value;
        if (!string.IsNullOrWhiteSpace(configuration.VapidPublicKey) &&
            !string.IsNullOrWhiteSpace(configuration.VapidPrivateKey))
        {
            _client.DefaultAuthentication = new VapidAuthentication(
                configuration.VapidPublicKey,
                configuration.VapidPrivateKey)
            {
                Subject = configuration.VapidSubject
            };
        }
    }

    public async Task<PushSendResult> SendAsync(
        PushTarget target,
        PushPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (_client.DefaultAuthentication is null)
        {
            _logger.LogWarning("Push delivery skipped because VAPID keys are not configured.");
            return PushSendResult.Failed;
        }

        var subscription = new WebPushSubscription
        {
            Endpoint = target.Endpoint,
            Keys = new Dictionary<string, string>
            {
                ["p256dh"] = target.P256dh,
                ["auth"] = target.Auth
            }
        };
        var json = JsonSerializer.Serialize(new
        {
            title = payload.Title,
            body = payload.Body,
            url = payload.Url,
            tag = payload.Tag
        });

        try
        {
            await _client.RequestPushMessageDeliveryAsync(
                subscription,
                new PushMessage(json),
                cancellationToken);
            return PushSendResult.Sent;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PushServiceClientException exception)
            when (exception.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone)
        {
            return PushSendResult.Gone;
        }
        catch (Exception exception)
        {
            var endpointHost = Uri.TryCreate(target.Endpoint, UriKind.Absolute, out var endpoint)
                ? endpoint.Host
                : "unknown";
            _logger.LogWarning(exception, "Push send failed for endpoint host {EndpointHost}", endpointHost);
            return PushSendResult.Failed;
        }
    }
}
