using FastEndpoints;
using GameManager.Application.Features.Games.Commands.EndTurn;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Games;

public class ActionEndTurnEndpoint : EndpointWithoutRequest<Results<NoContent, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public ActionEndTurnEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("{Id}/EndTurn");
        Group<GamesGroup>();
        Description(b =>
        {
            b.Produces(StatusCodes.Status404NotFound);
        });
        Policy(pol =>
        {
            pol.CanViewGame();
        });
        Version(1);
    }

    public override async Task<Results<NoContent, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new EndTurnCommand(id), ct);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.ToProblemDetails();
    }
}