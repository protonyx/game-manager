using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Mappers;
using GameManager.Server.Models;
using GreenDonut;

namespace GameManager.Server.DataLoaders;

public class TrackerByIdDataLoader : BatchDataLoader<Guid, GameTrackerModel>
{
    private readonly ITrackerRepository _trackerRepository;

    private readonly GraphQlMapper _mapper;

    public TrackerByIdDataLoader(
        ITrackerRepository trackerRepository,
        GraphQlMapper mapper,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options)
        : base(batchScheduler, options)
    {
        _trackerRepository = trackerRepository;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<Guid, GameTrackerModel>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        var result = await _trackerRepository.GetManyByIdAsync(keys, cancellationToken);
        return result.Select(_mapper.TrackerToModel).ToDictionary(t => t.Id);
    }
}