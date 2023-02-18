namespace GameManager.Server.Data;

public class PlayerRepository
{
    private readonly GameContext _context;

    public PlayerRepository(GameContext context)
    {
        _context = context;
    }
    
    
}