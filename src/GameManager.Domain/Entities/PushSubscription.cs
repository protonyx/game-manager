using GameManager.Domain.Common;

namespace GameManager.Domain.Entities;

public class PushSubscription : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid PlayerId { get; private set; }
    public Guid GameId { get; private set; }
    public string Endpoint { get; private set; }
    public string P256dh { get; private set; }
    public string Auth { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? LastNotifiedDate { get; private set; }

    public PushSubscription(Guid playerId, Guid gameId, string endpoint, string p256dh, string auth)
    {
        Id = Guid.NewGuid();
        PlayerId = playerId;
        GameId = gameId;
        Endpoint = endpoint;
        P256dh = p256dh;
        Auth = auth;
        CreatedDate = DateTime.UtcNow;
    }

    public void MarkNotified() => LastNotifiedDate = DateTime.UtcNow;
}
