using AutoMapper;
using GameManager.Server.Data;
using GameManager.Server.DTO;
using GameManager.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly GameRepository _gameRepository;

    private readonly IMapper _mapper;

    public GamesController(GameRepository gameRepository, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
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
    public async Task<IActionResult> JoinGame([FromQuery] string entryCode)
    {
        var game = await _gameRepository.GetGameByEntryCode(entryCode);
        
        if (game == null)
        {
            return NotFound();
        }

        throw new NotImplementedException();
    }

}