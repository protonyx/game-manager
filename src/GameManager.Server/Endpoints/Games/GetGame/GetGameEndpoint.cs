using CSharpFunctionalExtensions;
using FastEndpoints;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetGame;
using GameManager.Domain.ValueObjects;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints;

/// <summary>
/// Get Game by ID
/// </summary>
public class GetGameEndpoint : EndpointWithoutRequest<
    Results<
        Ok<GameDTO>,
        StatusCodeHttpResult,
        ProblemDetails>>
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
        Description(b =>
        {
            b.Produces(StatusCodes.Status404NotFound);
            b.Produces(StatusCodes.Status304NotModified);
        });
        Policy(pol =>
        {
            pol.CanViewGame();
        });
        Version(1);
    }

    public override async Task<Results<Ok<GameDTO>, StatusCodeHttpResult, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetGameQuery(id), ct);

        if (result.IsSuccess)
        {
            if (HttpContext.Request.Headers.IfNoneMatch.Count > 0)
            {
                var check = ETag.From(HttpContext.Request.Headers.IfNoneMatch[0]!);
                if (check.Equals(result.Value.ETag))
                {
                    return TypedResults.StatusCode(StatusCodes.Status304NotModified);
                }
            }
            HttpContext.Response.SetETag(result.Value.ETag);

            return TypedResults.Ok(result.Value.Game);
        }

        return result.Error.ToProblemDetails();
    }
}