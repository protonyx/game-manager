using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameManager.Persistence.Sqlite;

public class GameContextDesignTimeFactory : IDesignTimeDbContextFactory<GameContext>
{
    public GameContext CreateDbContext(string[] args)
    {
        var cs = "Data Source=gm.db";

        var opt = new DbContextOptionsBuilder<GameContext>();
        opt.UseSqlite(cs);

        return new GameContext(opt.Options);
    }
}