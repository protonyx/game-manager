using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Mappers;
using GameManager.Server.Models;
using GreenDonut;

namespace GameManager.Server.DataLoaders;

public class PlayerByIdDataLoader : BatchDataLoader<Guid, PlayerModel>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly GraphQlMapper _mapper;

    public PlayerByIdDataLoader(
        IPlayerRepository playerRepository,
        GraphQlMapper mapper,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options)
        : base(batchScheduler, options)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<Guid, PlayerModel>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var players = await _playerRepository.GetManyByIdAsync(keys, cancellationToken);

        return players.Select(_mapper.PlayerToModel).ToDictionary(p => p.Id);
    }
}