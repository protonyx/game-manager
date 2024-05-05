using FastEndpoints;
using GameManager.Application.Features.Games.Commands.ReorderPlayers;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Games;

public class ActionReorderPlayersEndpoint : Endpoint<PlayerIdListDTO, Results<NoContent, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public ActionReorderPlayersEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("{Id}/Reorder");
        Group<GamesGroup>();
        Description(b =>
        {
            b.Produces(StatusCodes.Status404NotFound);
        });
        Policy(pol =>
        {
            pol.CanModifyGame();
        });
        Version(1);
    }

    public override async Task<Results<NoContent, ProblemDetails>> ExecuteAsync(PlayerIdListDTO req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new ReorderPlayersCommand(id, req.PlayerIds), ct);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.ToProblemDetails();
    }
}