using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameManager.Persistence.Sqlite;

public class GameContextDesignTimeFactory : IDesignTimeDbContextFactory<GameContext>
{
    public GameContext CreateDbContext(string[] args)
    {
        var cs = "Data Source=gm.db";

        return new GameContext(cs, false);
    }
}