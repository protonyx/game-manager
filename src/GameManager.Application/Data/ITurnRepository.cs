using GameManager.Domain.Entities;

namespace GameManager.Application.Data;

public interface ITurnRepository : IAsyncRepository<Turn>
{
    Task<ICollection<Turn>> GetTurnsByPlayerId(Guid playerId);
}