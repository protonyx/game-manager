using AutoMapper;
using GameManager.Server.Data;
using GameManager.Server.DTO;
using GameManager.Server.Messages;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server.Services;

public class GameStateService
{
    private readonly GameRepository _gameRepository;

    private readonly PlayerRepository _playerRepository;
    
    private readonly IMapper _mapper;

    private readonly IHubContext<GameHub> _hubContext;

    public GameStateService(GameRepository gameRepository, PlayerRepository playerRepository, IMapper mapper, IHubContext<GameHub> hubContext)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task UpdatePlayerHeartbeat(Guid playerId)
    {
        await _playerRepository.UpdatePlayerHeartbeat(playerId);
    }

    public async Task<Guid?> GetCurrentTurn(Guid gameId)
    {
        var game = await _gameRepository.GetGameById(gameId);

        return game?.CurrentTurnPlayerId;
    }

    public async Task AdvanceTurn(Guid gameId)
    {
        var game = await _gameRepository.GetGameById(gameId);

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

        game = await _gameRepository.UpdateGameCurrentTurnAsync(game.Id, nextPlayer.Id);

        if (game == null)
        {
            return;
        }
        
        // Notify players
        var gameUpdatedMessage = new GameStateChangedMessage()
        {
            GameId = game.Id,
            Game = _mapper.Map<GameDTO>(game)
        };
            
        await _hubContext.Clients.Group(game.Id.ToString())
            .SendAsync(nameof(IGameHubClient.GameStateChanged), gameUpdatedMessage);
    }
}