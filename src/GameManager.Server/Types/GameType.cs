using GameManager.Application.Contracts.Persistence;
using GameManager.Server.Authorization;
using GameManager.Server.DataLoaders;
using GameManager.Server.Models;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;

namespace GameManager.Server.Types;

public class GameType : ObjectType<GameModel>
{
    protected override void Configure(IObjectTypeDescriptor<GameModel> descriptor)
    {
        descriptor.Field(t => t.CurrentTurnPlayer)
            .ResolveWith<GameResolvers>(t => t.GetCurrentPlayerAsync(default!, default!, default!))
            .Name("currentTurnPlayer");
        
        descriptor.Field(t => t.Players)
            .ResolveWith<GameResolvers>(t => t.GetPlayersAsync(default!,  default!, default!, default!))
            .Name("players");
    }

    private class GameResolvers
    {
        public async Task<IReadOnlyList<PlayerModel>> GetPlayersAsync(
            [Parent] GameModel game,
            IPlayerRepository playerRepository,
            PlayerByIdDataLoader playerById,
            CancellationToken cancellationToken)
        {
            var playerIds = await playerRepository.GetIdsByGameAsync(game.Id, cancellationToken);

            return await playerById.LoadAsync(playerIds, cancellationToken);
        }

        public async Task<PlayerModel?> GetCurrentPlayerAsync(
            [Parent] GameModel game,
            PlayerByIdDataLoader playerById,
            CancellationToken cancellationToken)
        {
            return game.CurrentTurnPlayerId.HasValue
                ? await playerById.LoadAsync(game.CurrentTurnPlayerId!.Value, cancellationToken)
                : null;
        }
    }
}