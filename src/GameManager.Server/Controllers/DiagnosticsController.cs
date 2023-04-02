using GameManager.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiagnosticsController : ControllerBase
{
    private readonly GameContext _context;

    public DiagnosticsController(GameContext context)
    {
        _context = context;
    }

    [HttpPost("Migrate")]
    public async Task<IActionResult> RunDatabaseMigrations()
    {
        await _context.Database.MigrateAsync();

        return NoContent();
    }
}