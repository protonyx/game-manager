using GameManager.Application.Data;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.EndTurn;

public class EndTurnCommandHandler : IRequestHandler<EndTurnCommand, EndTurnCommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    public EndTurnCommandHandler(IGameRepository gameRepository, IPlayerRepository playerRepository)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
    }

    public async Task<EndTurnCommandResponse> Handle(EndTurnCommand request, CancellationToken cancellationToken)
    {
        var ret = new EndTurnCommandResponse();
        
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return ret;
        }

        ret.ActionAllowed = true;
        
        var players = await _playerRepository.GetPlayersByGameId(request.GameId);
        var requestPlayer = players.First(t => t.Id == request.PlayerId);

        if (game.CurrentTurnPlayerId == null)
        {
            var firstPlayer = players.First();
            
            await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, firstPlayer.Id);
        }
        else
        {
            var currentPlayer = players.First(t => t.Id == game.CurrentTurnPlayerId);

            if (requestPlayer != currentPlayer && !requestPlayer.IsAdmin)
            {
                // Only the current player can end the turn
                ret.ActionAllowed = false;

                return ret;
            }
            
            var nextPlayer = players.FirstOrDefault(t => t.Order > currentPlayer.Order) ?? players.First();
            
            await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, nextPlayer.Id);
        }

        return ret;
    }
}