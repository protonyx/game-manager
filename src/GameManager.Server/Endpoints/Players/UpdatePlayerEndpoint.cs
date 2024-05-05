using FastEndpoints;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Players;

public class UpdatePlayerEndpoint : Endpoint<PlayerDTO, Results<Ok<PlayerDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public UpdatePlayerEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("{Id}");
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

    public override async Task<Results<Ok<PlayerDTO>, ProblemDetails>> ExecuteAsync(PlayerDTO req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new UpdatePlayerCommand(id, req), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}