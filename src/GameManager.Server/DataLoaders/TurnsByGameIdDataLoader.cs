using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Models;

namespace GameManager.Server.DataLoaders;

public class TurnsByGameIdDataLoader : BatchDataLoader<Guid, TurnModel[]>
{
    private readonly ITurnRepository _turnRepository;

    private readonly IMapper _mapper;

    public TurnsByGameIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        ITurnRepository turnRepository,
        IMapper mapper)
        : base(batchScheduler, options)
    {
        _turnRepository = turnRepository;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<Guid, TurnModel[]>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var result = new Dictionary<Guid, TurnModel[]>();

        foreach (var gameId in keys)
        {
            result[gameId] = (await _turnRepository.GetTurnsByGameId(gameId, cancellationToken))
                .Select(_mapper.Map<TurnModel>)
                .ToArray();
        }

        return result;
    }
}