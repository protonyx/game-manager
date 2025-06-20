using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Commands.DeletePlayer;

public class DeletePlayerCommand : ICommand
{
    public Guid PlayerId { get; }

    public DeletePlayerCommand(Guid playerId)
    {
        PlayerId = playerId;
    }
}