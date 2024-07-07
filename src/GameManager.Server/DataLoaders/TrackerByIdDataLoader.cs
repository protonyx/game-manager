using AutoMapper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Models;
using GreenDonut;

namespace GameManager.Server.DataLoaders;

public class TrackerByIdDataLoader : BatchDataLoader<Guid, GameTrackerModel>
{
    private readonly ITrackerRepository _trackerRepository;

    private readonly IMapper _mapper;
    
    public TrackerByIdDataLoader(
        IBatchScheduler batchScheduler,
        ITrackerRepository trackerRepository,
        IMapper mapper)
        : base(batchScheduler)
    {
        _trackerRepository = trackerRepository;
        _mapper = mapper;
    }

    protected override async Task<IReadOnlyDictionary<Guid, GameTrackerModel>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        var result = await _trackerRepository.GetManyByIdAsync(keys, cancellationToken);
        return result.Select(_mapper.Map<GameTrackerModel>).ToDictionary(t => t.Id);
    }
}