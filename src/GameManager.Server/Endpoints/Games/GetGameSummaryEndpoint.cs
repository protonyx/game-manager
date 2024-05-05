using FastEndpoints;
using GameManager.Application.Features.Games.Queries.GetGameSummary;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Games;

public class GetGameSummaryEndpoint : EndpointWithoutRequest<Results<Ok<GameSummaryDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public GetGameSummaryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("{Id}/Summary");
        Group<GamesGroup>();
        Description(b =>
        {
            b.Produces(StatusCodes.Status404NotFound);
        });
        AllowAnonymous();
        Version(1);
    }

    public override async Task<Results<Ok<GameSummaryDTO>, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetGameSummaryQuery(id), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}