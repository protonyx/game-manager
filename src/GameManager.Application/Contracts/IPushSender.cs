namespace GameManager.Application.Contracts;

public record PushTarget(string Endpoint, string P256dh, string Auth);

public record PushPayload(string Title, string Body, string Url, string? Tag = null);

public enum PushSendResult
{
    Sent,
    Gone,
    Failed
}

public interface IPushSender
{
    Task<PushSendResult> SendAsync(
        PushTarget target,
        PushPayload payload,
        CancellationToken cancellationToken = default);
}
