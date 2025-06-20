using FastEndpoints;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayerList;
using GameManager.Server.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace GameManager.Server.Endpoints.Games;

public class GetGamePlayersEndpoint : EndpointWithoutRequest<Results<Ok<IReadOnlyList<PlayerDTO>>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public GetGamePlayersEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("{Id}/Players");
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

    public override async Task<Results<Ok<IReadOnlyList<PlayerDTO>>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetPlayerListQuery(id), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}