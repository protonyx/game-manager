using GameManager.Application.Commands;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.Commands.ReorderPlayers;
using GameManager.Application.Features.Games.DTO;
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
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGame(
        [FromBody] CreateGameCommand game,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(game, cancellationToken);

        if (response is EntityCommandResponse entity)
        {
            return CreatedAtAction(nameof(GetGame), 
                new {id = entity.Id},
                entity.Value);
        }

        return this.GetActionResult(response);
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

    [HttpPost("{id}/Actions/Reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize]
    public async Task<IActionResult> ReorderPlayers(
        [FromRoute] Guid id,
        [FromBody] PlayerIdListDTO playerIdList,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new ReorderPlayersCommand(id, playerIdList.PlayerIds), cancellationToken);

        return NoContent();
    }

    [HttpPost("{id}/Actions/EndTurn")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [Authorize]
    public async Task<IActionResult> EndTurn(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new EndTurnCommand(id), cancellationToken);

        return this.GetActionResult(response);
    }

    [HttpPost("Join")]
    [ProducesResponseType(typeof(PlayerCredentialsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinGame(
        [FromBody] JoinGameCommand player,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(player, cancellationToken);

        return this.GetActionResult(response);
    }

}