using GameManager.Application.DTO;
using GameManager.Application.Features.Games.Commands;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.Queries.GetGame;
using GameManager.Application.Features.Games.Queries.GetPlayerList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GameDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGame(
        [FromBody] CreateGameCommand game,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(game, cancellationToken);
        
        if (response.ValidationResult is {IsValid: false})
        {
            // TODO: Format response
            return BadRequest(response.ValidationResult);
        }

        return CreatedAtAction(nameof(GetGame), 
            new {id = response.Game!.Id},
            response.Game);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GameDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetGame([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new GetGameQuery(id));

        if (response == null)
        {
            return NotFound();
        }
        
        return Ok(response);
    }

    [HttpGet("{id}/Players")]
    [ProducesResponseType(typeof(ICollection<PlayerDTO>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetGamePlayers([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new GetPlayerListQuery(id));
        
        return Ok(response);
    }

    [HttpPost("Join")]
    [ProducesResponseType(typeof(JoinGameCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinGame(
        [FromBody] JoinGameCommand player,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(player, cancellationToken);

        if (response.ValidationResult is {IsValid: false})
        {
            // TODO: Format response
            return BadRequest(response.ValidationResult);
        }

        return Ok(response);
    }

}