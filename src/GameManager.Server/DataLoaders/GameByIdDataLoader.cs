using AutoMapper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Models;
using GreenDonut;

namespace GameManager.Server.DataLoaders;

public class GameByIdDataLoader : BatchDataLoader<Guid, GameModel>
{
    private readonly IGameRepository _gameRepository;

    private readonly IMapper _mapper;
    
    public GameByIdDataLoader(
        IGameRepository gameRepository,
        IMapper mapper,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options)
        : base(batchScheduler, options)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<Guid, GameModel>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var result = await _gameRepository.GetManyByIdAsync(keys, cancellationToken);
        return result.Select(_mapper.Map<GameModel>).ToDictionary(t => t.Id);
    }
}