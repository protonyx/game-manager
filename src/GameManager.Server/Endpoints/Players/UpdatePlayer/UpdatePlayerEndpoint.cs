using FastEndpoints;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Server.Authorization;

namespace GameManager.Server.Endpoints.Players;

public class UpdatePlayerEndpoint(IMediator mediator)
    : Endpoint<UpdatePlayerDTO, Results<Ok<PlayerDTO>, ProblemDetails>>
{
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

    public override async Task<Results<Ok<PlayerDTO>, ProblemDetails>> ExecuteAsync(UpdatePlayerDTO req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await mediator.Send(new UpdatePlayerCommand(id, req), ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}