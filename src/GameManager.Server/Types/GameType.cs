using GameManager.Server.DataLoaders;
using GameManager.Server.Models;

namespace GameManager.Server.Types;

public class GameType : ObjectType<GameModel>
{
    protected override void Configure(IObjectTypeDescriptor<GameModel> descriptor)
    {
        descriptor.Field(t => t.CurrentTurnPlayer)
            .Resolve((ctx, ct) =>
            {
                var parent = ctx.Parent<GameModel>();
                var dataLoader = ctx.Services.GetRequiredService<PlayerByIdDataLoader>();
                return dataLoader.LoadAsync(parent.CurrentTurnPlayerId!.Value, ct);
            })
            .Name("currentTurnPlayer");

        descriptor.Field(t => t.Players)
            .Resolve((ctx, ct) =>
            {
                var parent = ctx.Parent<GameModel>();
                var dataLoader = ctx.Services.GetRequiredService<PlayersByGameIdDataLoader>();

                return dataLoader.LoadAsync(parent.Id, ct);
            })
            .Name("players");

        descriptor.Field(t => t.Turns)
            .Resolve((ctx, ct) =>
            {
                var parent = ctx.Parent<GameModel>();
                var dataLoader = ctx.Services.GetRequiredService<TurnsByGameIdDataLoader>();
                return dataLoader.LoadAsync(parent.Id, ct);
            });
    }
}