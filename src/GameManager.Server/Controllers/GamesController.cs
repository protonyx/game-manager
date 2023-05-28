using System.ComponentModel.DataAnnotations;
using AutoMapper;
using GameManager.Application.Data;
using GameManager.Application.Features.Games.Commands;
using GameManager.Server.Data;
using GameManager.Server.DTO;
using GameManager.Server.Models;
using GameManager.Server.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    private readonly IMediator _mediator;

    public GamesController(
        IGameRepository gameRepository, 
        IPlayerRepository playerRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
        _playerRepository = playerRepository;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] NewGameDTO game)
    {
        var model = _mapper.Map<Game>(game);

        var newGame = await _gameRepository.CreateAsync(model);

        var ret = _mapper.Map<GameDTO>(newGame);

        return Ok(ret);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetGame([FromRoute] Guid id)
    {
        var game = await _gameRepository.GetByIdAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        if (!await VerifyActivePlayerAsync())
        {
            return Forbid();
        }

        var dto = _mapper.Map<GameDTO>(game);

        return Ok(dto);
    }

    [HttpGet("{id}/Players")]
    [Authorize]
    public async Task<IActionResult> GetGamePlayers([FromRoute] Guid id)
    {
        var game = await _gameRepository.GetByIdAsync(id);

        if (game == null)
        {
            return NotFound();
        }
        
        if (!await VerifyActivePlayerAsync())
        {
            return Forbid();
        }

        var players = await _playerRepository.GetPlayersByGameId(id);

        var ret = _mapper.Map<ICollection<PlayerDTO>>(players);
        
        return Ok(ret);
    }

    [HttpPost("Join")]
    [ProducesResponseType(typeof(PlayerCredentialsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinGame(
        [FromBody] JoinGameCommand player,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(player, cancellationToken);

        if (response.ValidationResults.Any())
        {
            foreach (var validationResult in response.ValidationResults)
            {
                ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
            
            return BadRequest(ModelState);
        }

        return Ok(response.Credentials);
    }

    private async Task<bool> VerifyActivePlayerAsync()
    {
        // Check that the user represents an active player
        var playerId = User.GetPlayerId();

        if (!playerId.HasValue)
        {
            return false;
        }

        var player = await _playerRepository.GetByIdAsync(playerId.Value);

        return player is {Active: true};
    }

}