using FastEndpoints;
using GameManager.Application.Features.Games.Commands.EndGame;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Games;

public class ActionEndGameEndpoint : EndpointWithoutRequest<Results<NoContent, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public ActionEndGameEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override void Configure()
    {
        Post("{Id}/End");
        Group<GamesGroup>();
        Policy(pol =>
        {
            pol.CanModifyGame();
        });
    }

    public override async Task<Results<NoContent, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new EndGameCommand(id), ct);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.ToProblemDetails();
    }
}