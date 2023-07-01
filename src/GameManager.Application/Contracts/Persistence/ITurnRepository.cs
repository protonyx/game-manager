using GameManager.Domain.Entities;

namespace GameManager.Application.Contracts.Persistence;

public interface ITurnRepository : IAsyncRepository<Turn>
{
    Task<ICollection<Turn>> GetTurnsByPlayerId(Guid playerId);
}