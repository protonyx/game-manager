using FastEndpoints;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetGame;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameManager.Server.Endpoints;

public class GetGameEndpoint : EndpointWithoutRequest<Results<Ok<GameDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public GetGameEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("{Id}");
        Group<GamesGroup>();
    }

    public override async Task<Results<Ok<GameDTO>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetGameQuery(id), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}