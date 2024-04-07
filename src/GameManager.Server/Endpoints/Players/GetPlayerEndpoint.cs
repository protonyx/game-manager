using FastEndpoints;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayer;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Players;

public class GetPlayerEndpoint : EndpointWithoutRequest<Results<Ok<PlayerDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public GetPlayerEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("{Id}");
        Policy(pol =>
        {
            pol.CanViewPlayer();
        });
        Group<PlayersGroup>();
    }

    public override async Task<Results<Ok<PlayerDTO>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetPlayerQuery(id), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}