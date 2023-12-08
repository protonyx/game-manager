using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Application.Features.Games.Commands.EndGame;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.Commands.ReorderPlayers;
using GameManager.Application.Features.Games.Commands.StartGame;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetGame;
using GameManager.Application.Features.Games.Queries.GetGameSummary;
using GameManager.Application.Features.Games.Queries.GetPlayerList;
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
        [FromBody] CreateGameDTO game,
        CancellationToken cancellationToken)
    {
        var request = new CreateGameCommand(game);
        
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetGame),
                new {id = result.Value.Id},
                result.Value)
            : this.GetErrorActionResult(result.Error);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GameDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetGame([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetGameQuery(id));

        return result.IsSuccess ? Ok(result.Value) : this.GetErrorActionResult(result.Error);
    }

    [HttpGet("{id}/Players")]
    [ProducesResponseType(typeof(ICollection<PlayerDTO>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetGamePlayers([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetPlayerListQuery(id));

        return result.IsSuccess ? Ok(result.Value) : this.GetErrorActionResult(result.Error);
    }

    [HttpPost("{id}/Actions/Reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize]
    public async Task<IActionResult> ReorderPlayers(
        [FromRoute] Guid id,
        [FromBody] PlayerIdListDTO playerIdList,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ReorderPlayersCommand(id, playerIdList.PlayerIds), cancellationToken);

        return result.IsSuccess ? NoContent() : this.GetErrorActionResult(result.Error);
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
        var result = await _mediator.Send(new EndTurnCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : this.GetErrorActionResult(result.Error);
    }
    
    [HttpPost("{id}/Actions/Start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [Authorize]
    public async Task<IActionResult> StartGame(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartGameCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : this.GetErrorActionResult(result.Error);
    }
    
    [HttpPost("{id}/Actions/Complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [Authorize]
    public async Task<IActionResult> EndGame(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EndGameCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : this.GetErrorActionResult(result.Error);
    }

    [HttpPost("Join")]
    [ProducesResponseType(typeof(PlayerCredentialsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinGame(
        [FromBody] JoinGameDTO player,
        CancellationToken cancellationToken)
    {
        var request = new JoinGameCommand(player.EntryCode, player.Name);
        
        var result = await _mediator.Send(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : this.GetErrorActionResult(result.Error);
    }

    [HttpGet("{id}/Summary")]
    [ProducesResponseType(typeof(GameSummaryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameSummary(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetGameSummaryQuery(id);
        
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : this.GetErrorActionResult(result.Error);
    }

}