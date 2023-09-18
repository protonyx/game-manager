using GameManager.Domain.ValueObjects;

namespace GameManager.Domain.Entities;

public class Player
{
    private readonly Guid _id;
    public Guid Id => _id;

    private readonly Guid _gameId;
    public Guid GameId => _gameId;

    private int _order;
    public int Order => _order;

    private string _name;
    public PlayerName Name
    {
        get => PlayerName.Of(_name);
        set => _name = value.Value;
    }

    private bool _active;
    public bool Active => _active;

    private bool _isAdmin;
    public bool IsAdmin => _isAdmin;

    private DateTime _joinedDate;
    public DateTime JoinedDate => _joinedDate;

    private DateTime? _lastHeartbeat;
    public DateTime? LastHeartbeat => _lastHeartbeat;

    public ICollection<TrackerValue> TrackerValues { get; set; } = new List<TrackerValue>();

    public ICollection<Turn> Turns { get; set; } = new List<Turn>();
    
    public ICollection<TrackerHistory> TrackerHistory { get; set; } = new List<TrackerHistory>();

    protected Player()
    {
    }
    
    public Player(PlayerName name, Game game)
        :this()
    {
        _id = Guid.NewGuid();
        _active = true;
        _isAdmin = false;
        
        _name = name.Value;
        _gameId = game.Id;
        _joinedDate = DateTime.UtcNow;
    }
    
    public void UpdateHeartbeat()
    {
        _lastHeartbeat = DateTime.UtcNow;
    }

    public void Promote()
    {
        _isAdmin = true;
    }

    public void SetOrder(int newOrder)
    {
        if (newOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(newOrder), "Order cannot be negative");

        _order = newOrder;
    }

    public void SoftDelete()
    {
        _active = false;
        _order = 0;
    }
}