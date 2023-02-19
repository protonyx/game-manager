using GameManager.Server.Data;

namespace GameManager.Server.Services;

public class GameStateService
{
    private readonly GameRepository _gameRepository;

    private readonly PlayerRepository _playerRepository;

    public GameStateService(GameRepository gameRepository, PlayerRepository playerRepository)
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
        var game = await _gameRepository.GetGameById(gameId, false);

        return game?.CurrentTurnPlayerId;
    }

    public async Task AdvanceTurn(Guid gameId)
    {
        var game = await _gameRepository.GetGameById(gameId, true);

        if (game == null || !game.Players.Any())
        {
            return;
        }
        
        var firstPlayer = game.Players.OrderBy(t => t.Order).First();

        if (game.CurrentTurnPlayerId == null)
        {
            await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, firstPlayer.Id);

            return;
        }
        
        var currentPlayer = game.Players.FirstOrDefault(t => t.Id == game.CurrentTurnPlayerId);

        if (currentPlayer == null)
        {
            await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, firstPlayer.Id);

            return;
        }

        var nextPlayer = game.Players
            .OrderBy(t => t.Order)
            .FirstOrDefault(t => t.Order > currentPlayer.Order);

        if (nextPlayer == null)
        {
            await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, firstPlayer.Id);

            return;
        }

        await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, nextPlayer.Id);
    }
}