using GameManager.Server.Models;

namespace GameManager.Application.Data;

public interface IGameRepository : IAsyncRepository<Game>
{
    Task<Game?> UpdateGameCurrentTurnAsync(Guid id, Guid playerId);
    Task<Game?> GetGameByEntryCodeAsync(string entryCode);
}