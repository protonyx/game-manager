using GameManager.Application.Commands;
using GameManager.Application.Data;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommandHandler : IRequestHandler<EndTurnCommand, ICommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly ITurnRepository _turnRepository;

    public EndTurnCommandHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        ITurnRepository turnRepository)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _turnRepository = turnRepository;
    }

    public async Task<ICommandResponse> Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return CommandResponses.NotFound();
        }
        
        var players = await _playerRepository.GetPlayersByGameIdAsync(request.GameId);
        var requestPlayer = players.First(t => t.Id == request.PlayerId);

        if (game.CurrentTurnPlayerId == null)
        {
            var firstPlayer = players.First();

            game.CurrentTurnPlayerId = firstPlayer.Id;
        }
        else
        {
            var currentPlayer = players.First(t => t.Id == game.CurrentTurnPlayerId);

            if (requestPlayer != currentPlayer && !requestPlayer.IsAdmin)
            {
                // Only the current player can end the turn
                return CommandResponses.AuthorizationError();
            }
            
            var nextPlayer = players.FirstOrDefault(t => t.Order > currentPlayer.Order) ?? players.First();

            var turn = new Turn()
            {
                PlayerId = currentPlayer.Id,
                StartTime = game.LastTurnStartTime ?? DateTime.Now,
                EndTime = DateTime.Now
            };
            turn.Duration = turn.EndTime - turn.StartTime;

            await _turnRepository.CreateAsync(turn);

            game.CurrentTurnPlayerId = nextPlayer.Id;
        }

        game.LastTurnStartTime = DateTime.Now;
        await _gameRepository.UpdateAsync(game);

        return CommandResponses.Success();
    }
}