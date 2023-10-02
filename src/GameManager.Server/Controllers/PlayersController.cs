using System.ComponentModel.DataAnnotations;
using GameManager.Application.Features.Games.Commands.DeletePlayer;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayer;
using GameManager.Application.Features.Games.Queries.GetPlayerTurns;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PlayersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PlayerDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayer([FromRoute] Guid id)
    {
        var player = await _mediator.Send(new GetPlayerQuery(id));

        if (player == null)
        {
            return NotFound();
        }

        return Ok(player);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PlayerDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePlayer(
        [FromRoute] Guid id,
        [FromBody] PlayerDTO dto)
    {
        var result = await _mediator.Send(new UpdatePlayerCommand(id, dto));

        return result.IsSuccess ? Ok(result.Value) : this.GetErrorActionResult(result.Error);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(PlayerDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchPlayer(
        [FromRoute] Guid id,
        [FromBody, Required] JsonPatchDocument<PlayerDTO> patchDoc)
    {
        var player = await _mediator.Send(new GetPlayerQuery(id));

        if (player == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(player, ModelState);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updateResult = await _mediator.Send(new UpdatePlayerCommand(id, player));

        return updateResult.IsSuccess ? Ok(updateResult.Value) : this.GetErrorActionResult(updateResult.Error);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePlayer([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeletePlayerCommand(id));

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return this.GetErrorActionResult(result.Error);
    }

    [HttpGet("{id}/Turns")]
    [ProducesResponseType(typeof(ICollection<TurnDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlayerTurns([FromRoute] Guid id)
    {
        var turns = await _mediator.Send(new GetPlayerTurnsQuery(id));

        return Ok(turns);
    }
}