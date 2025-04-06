using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Models;

namespace GameManager.Server.DataLoaders;

public class PlayersByGameIdDataLoader : BatchDataLoader<Guid, PlayerModel[]>
{
    private readonly IPlayerRepository _repository;
    
    private readonly IMapper _mapper;

    public PlayersByGameIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IPlayerRepository repository,
        IMapper mapper)
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
                .Select(_mapper.Map<PlayerModel>)
                .ToArray();
        }

        return result;
    }
}