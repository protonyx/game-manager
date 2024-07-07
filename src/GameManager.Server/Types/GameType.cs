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
        descriptor.Authorize(AuthorizationPolicyNames.ViewGame, ApplyPolicy.AfterResolver);
        
        descriptor.Field(t => t.Players)
            .ResolveWith<GameResolvers>(t => t.GetPlayersAsync(default!,  default!, default!, default!))
            .Name("players");
    }

    private class GameResolvers
    {
        public async Task<IReadOnlyList<PlayerModel>> GetPlayersAsync(
            [Parent] GameModel game,
            [Service] IPlayerRepository playerRepository,
            PlayerByIdDataLoader playerById,
            CancellationToken cancellationToken)
        {
            var playerIds = await playerRepository.GetIdsByGameAsync(game.Id, cancellationToken);

            return await playerById.LoadAsync(playerIds, cancellationToken);
        }
    }
}