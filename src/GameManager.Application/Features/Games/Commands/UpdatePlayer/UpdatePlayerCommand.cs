using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.UpdatePlayer;

public class UpdatePlayerCommand : ICommand<PlayerDTO>
{
    public Guid PlayerId { get; }

    public UpdatePlayerDTO Player { get; }

    public UpdatePlayerCommand(Guid playerId, UpdatePlayerDTO player)
    {
        PlayerId = playerId;
        Player = player;
    }
}