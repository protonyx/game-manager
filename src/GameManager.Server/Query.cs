using AutoMapper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Authorization;
using GameManager.Server.DataLoaders;
using GameManager.Server.Models;
using HotChocolate;
using HotChocolate.Authorization;

namespace GameManager.Server;

public class Query
{
    [Authorize(AuthorizationPolicyNames.Admin)]
    public async Task<IReadOnlyList<GameModel>> GetGamesAsync(
        [Service] IGameRepository gameRepository,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        (await gameRepository.FindAsync(DateTime.Now, cancellationToken))
        .Select(mapper.Map<GameModel>)
        .ToList();

    public async Task<GameModel?> GetGameAsync(
        Guid id,
        GameByIdDataLoader gameById,
        CancellationToken cancellationToken)
    {
        return await gameById.LoadAsync(id, cancellationToken);
    }
}