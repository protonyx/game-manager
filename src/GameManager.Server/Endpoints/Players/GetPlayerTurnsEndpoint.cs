using FastEndpoints;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayerTurns;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Players;

public class GetPlayerTurnsEndpoint : EndpointWithoutRequest<Results<Ok<IReadOnlyList<TurnDTO>>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public GetPlayerTurnsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("{Id}/Turns");
        Group<PlayersGroup>();
        Description(b =>
        {
            b.Produces(StatusCodes.Status404NotFound);
        });
        Policy(pol =>
        {
            pol.CanViewPlayer();
        });
        Version(1);
    }

    public override async Task<Results<Ok<IReadOnlyList<TurnDTO>>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetPlayerTurnsQuery(id), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}