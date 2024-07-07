using GameManager.Server.DataLoaders;
using GameManager.Server.Models;
using HotChocolate;
using HotChocolate.Types;

namespace GameManager.Server.Types;

public class PlayerTrackerValueType : ObjectType<PlayerTrackerValueModel>
{
    protected override void Configure(IObjectTypeDescriptor<PlayerTrackerValueModel> descriptor)
    {
        descriptor.Field(t => t.Name)
            .ResolveWith<PlayerTrackerValueResolvers>(
                t => t.GetTrackerNameAsync( default!, default!, default!))
            .Name("name");
    }

    private class PlayerTrackerValueResolvers
    {
        public async Task<string> GetTrackerNameAsync(
            [Parent] PlayerTrackerValueModel model,
            TrackerByIdDataLoader trackerById,
            CancellationToken cancellationToken)
        {
            var trackerId = model.TrackerId;
            var tracker = await trackerById.LoadAsync(trackerId, cancellationToken);

            return tracker.Name;
        }
    }
}