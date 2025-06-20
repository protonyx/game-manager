using FastEndpoints;
using GameManager.Application.Features.Games.Commands.DeletePlayer;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Players;

public class DeletePlayerEndpoint : EndpointWithoutRequest<Results<NoContent, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public DeletePlayerEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("{Id}");
        Group<PlayersGroup>();
        Description(b =>
        {
            b.Produces(StatusCodes.Status404NotFound);
        });
        Policy(pol =>
        {
            pol.CanModifyPlayer();
        });
        Version(1);
    }

    public override async Task<Results<NoContent, ProblemDetails>> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new DeletePlayerCommand(id), ct);

        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.ToProblemDetails();
    }
}