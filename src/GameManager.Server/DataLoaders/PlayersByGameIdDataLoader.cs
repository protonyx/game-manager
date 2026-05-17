using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Mappers;
using GameManager.Server.Models;

namespace GameManager.Server.DataLoaders;

public class PlayersByGameIdDataLoader : BatchDataLoader<Guid, PlayerModel[]>
{
    private readonly IPlayerRepository _repository;

    private readonly GraphQlMapper _mapper;

    public PlayersByGameIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IPlayerRepository repository,
        GraphQlMapper mapper)
        : base(batchScheduler, options)
    {
        _repository = repository;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<Guid, PlayerModel[]>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var result = new Dictionary<Guid, PlayerModel[]>();

        foreach (var gameId in keys)
        {
            result[gameId] = (await _repository.GetByGameIdAsync(gameId, cancellationToken))
                .Select(_mapper.PlayerToModel)
                .ToArray();
        }

        return result;
    }
}