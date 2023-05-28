using GameManager.Application.Data;

namespace GameManager.Server.Services;

public class GameStateService
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    public GameStateService(IGameRepository gameRepository, IPlayerRepository playerRepository)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
    }

    public async Task UpdatePlayerHeartbeat(Guid playerId)
    {
        await _playerRepository.UpdatePlayerHeartbeat(playerId);
    }

    public async Task<Guid?> GetCurrentTurn(Guid gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);

        return game?.CurrentTurnPlayerId;
    }

    public async Task AdvanceTurn(Guid gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);

        var players = await _playerRepository.GetPlayersByGameId(gameId);

        if (game == null || !players.Any())
        {
            return;
        }
        
        var currentPlayer = players.FirstOrDefault(t => t.Id == game.CurrentTurnPlayerId);
        var firstPlayer = players.FirstOrDefault();
        var nextPlayer = currentPlayer == null
            ? firstPlayer
            : players.FirstOrDefault(t => t.Order > currentPlayer.Order) ?? firstPlayer;

        if (nextPlayer == null)
        {
            return;
        }

        await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, nextPlayer.Id);
    }
}