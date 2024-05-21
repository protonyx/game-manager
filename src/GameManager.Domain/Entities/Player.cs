using GameManager.Domain.Common;
using GameManager.Domain.ValueObjects;

namespace GameManager.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }

    public Guid GameId { get; private set; }

    public int Order { get; private set; }

    public PlayerName Name { get; private set; }

    public bool Active { get; private set; }

    public bool IsHost { get; private set; }

    public DateTime JoinedDate { get; private set; }

    private List<PlayerConnection> _connections = new();

    public IReadOnlyList<PlayerConnection> Connections => _connections.ToList();

    private List<TrackerValue> _trackerValues = new();

    public IReadOnlyList<TrackerValue> TrackerValues => _trackerValues.ToList();

    protected Player()
    {
    }

    protected Player(Player player)
    {
        Id = player.Id;
        GameId = player.GameId;
        Order = player.Order;
        Name = player.Name;
        Active = player.Active;
        IsHost = player.IsHost;
        JoinedDate = player.JoinedDate;
        _trackerValues = player._trackerValues;
    }

    public Player(PlayerName name, Game game)
        : this()
    {
        Id = Guid.NewGuid();
        Active = true;
        IsHost = false;

        Name = name;
        GameId = game.Id;
        JoinedDate = DateTime.UtcNow;

        if (game.Trackers.Count > 0)
        {
            foreach (var tracker in game.Trackers)
            {
                SetTracker(tracker.Id, tracker.StartingValue);
            }
        }
    }

    public void UpdateHeartbeat(string connectionId)
    {
        var existing = _connections.FirstOrDefault(c => c.ConnectionId.Equals(connectionId));

        existing?.UpdateHeartbeat();
    }

    public void AddConnection(string connectionId)
    {
        if (!_connections.Any(c => c.ConnectionId.Equals(connectionId)))
        {
            _connections.Add(new PlayerConnection(Id, connectionId));
        }
    }

    public void RemoveConnection(string connectionId)
    {
        var existing = _connections.FirstOrDefault(c => c.ConnectionId.Equals(connectionId));

        if (existing != null)
        {
            _connections.Remove(existing);
        }
    }

    public void Promote()
    {
        IsHost = true;
    }

    public void SetName(PlayerName name)
    {
        Name = name;
    }

    public Result SetOrder(int newOrder)
    {
        if (newOrder < 0)
            return Result.Failure("Order cannot be negative");

        Order = newOrder;

        return Result.Success();
    }

    public Result SetTracker(Guid trackerId, int value)
    {
        var tracker = _trackerValues.FirstOrDefault(t => t.TrackerId == trackerId);

        if (tracker == null)
        {
            _trackerValues.Add(new TrackerValue()
            {
                Id = Guid.NewGuid(),
                PlayerId = Id,
                TrackerId = trackerId,
                Value = value
            });
        }
        else if (tracker.Value == value)
        {
            return Result.Failure("Tracker value did not change");
        }
        else
        {
            tracker.Value = value;
        }

        return Result.Success();
    }

    public void SoftDelete()
    {
        Active = false;
        Order = 0;
    }
}