using AutoMapper;
using GameManager.Server.Data;
using GameManager.Server.DTO;
using GameManager.Server.Messages;
using GameManager.Server.Models;
using GameManager.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly GameRepository _gameRepository;

    private readonly PlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    private readonly IHubContext<GameHub> _hubContext;

    private readonly TokenService _tokenService;

    public GamesController(
        GameRepository gameRepository, 
        PlayerRepository playerRepository,
        IMapper mapper,
        IHubContext<GameHub> hubContext,
        TokenService tokenService)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
        _playerRepository = playerRepository;
        _hubContext = hubContext;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] NewGameDTO game)
    {
        var model = _mapper.Map<Game>(game);

        var newGame = await _gameRepository.CreateGameAsync(model);

        var ret = _mapper.Map<GameDTO>(newGame);

        return Ok(ret);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetGame([FromRoute] Guid id)
    {
        var game = await _gameRepository.GetGameById(id);

        if (game == null)
        {
            return NotFound();
        }

        var dto = _mapper.Map<GameDTO>(game);

        return Ok(dto);
    }

    [HttpGet("{id}/Players")]
    [Authorize]
    public async Task<IActionResult> GetGamePlayers([FromRoute] Guid id)
    {
        var game = await _gameRepository.GetGameById(id, true);

        if (game == null)
        {
            return NotFound();
        }

        var players = _mapper.Map<ICollection<PlayerDTO>>(game.Players);
        
        return Ok(players);
    }

    [HttpPost("Join")]
    public async Task<IActionResult> JoinGame([FromBody] NewPlayerDTO player)
    {
        var game = await _gameRepository.GetGameByEntryCode(player.EntryCode);
        
        if (game == null)
        {
            return NotFound();
        }

        var newPlayer = new Player()
        {
            Name = player.Name
        };

        newPlayer = await _playerRepository.CreatePlayerAsync(game.Id, newPlayer);

        var dto = _mapper.Map<PlayerCredentialsDTO>(newPlayer);

        // Generate token
        dto.Token = _tokenService.GenerateToken(game.Id, newPlayer.Id, newPlayer.IsAdmin);

        // Notify other players
        await _hubContext.Clients.Group(game.Id.ToString())
            .SendAsync(nameof(IGameHubClient.PlayerJoined), new PlayerJoinedMessage()
            {
                GameId = game.Id,
                PlayerId = newPlayer.Id
            });

        return Ok(dto);
    }

}