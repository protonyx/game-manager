using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.Notifications.PlayerUpdated;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.ReorderPlayers;

public class ReorderPlayersCommandHandler : IRequestHandler<ReorderPlayersCommand>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMediator _mediator;

    public ReorderPlayersCommandHandler(IPlayerRepository playerRepository, IMediator mediator)
    {
        _playerRepository = playerRepository;
        _mediator = mediator;
    }

    public async Task Handle(ReorderPlayersCommand request, CancellationToken cancellationToken)
    {
        var playerIds = request.PlayerIds;

        var players = (await _playerRepository.GetPlayersByGameIdAsync(request.GameId)).ToList();
        
        // Rearrange players based on input list
        for (int i = 0; i < playerIds.Count; i++)
        {
            var player = players.FirstOrDefault(p => p.Id == playerIds[i]);

            if (player != null)
            {
                player.SetOrder(i + 1);
            }
        }

        await _playerRepository.UpdatePlayersAsync(players);

        foreach (var player in players)
        {
            await _mediator.Publish(new PlayerUpdatedNotification(player), cancellationToken);
        }
    }
}