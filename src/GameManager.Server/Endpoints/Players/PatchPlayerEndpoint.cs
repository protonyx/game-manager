using FastEndpoints;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayer;
using GameManager.Server.Authorization;
using GameManager.Server.DTO;

namespace GameManager.Server.Endpoints.Players;

public class PatchPlayerDTO : PatchRequest
{
    public Guid Id { get; set; }
}

public class PatchPlayerEndpoint : Endpoint<PatchPlayerDTO, Results<Ok<PlayerDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public PatchPlayerEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Patch("{Id}");
        Policy(pol =>
        {
            pol.CanModifyPlayer();
        });
        Group<PlayersGroup>();
    }

    public override async Task<Results<Ok<PlayerDTO>, ProblemDetails>> ExecuteAsync(PatchPlayerDTO req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetPlayerQuery(id), ct);

        if (result.IsFailure)
        {
            return result.Error.ToProblemDetails();
        }
        
        var player = result.Value;

        req.Patch(player);
        
        var updateResult = await _mediator.Send(new UpdatePlayerCommand(req.Id, player), ct);

        return updateResult.IsSuccess
            ? TypedResults.Ok(updateResult.Value)
            : updateResult.Error.ToProblemDetails();
    }
}