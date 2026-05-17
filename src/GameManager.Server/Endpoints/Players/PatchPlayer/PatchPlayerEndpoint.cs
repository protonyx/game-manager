using FastEndpoints;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetPlayer;
using GameManager.Application.Mappers;
using GameManager.Server.Authorization;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

namespace GameManager.Server.Endpoints.Players;

public class PatchPlayerEndpoint : Endpoint<PatchPlayerDTO, Results<Ok<PlayerDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    private readonly DtoMapper _mapper;

    public PatchPlayerEndpoint(IMediator mediator, DtoMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Patch("{Id}");
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

    public override async Task<Results<Ok<PlayerDTO>, ProblemDetails>> ExecuteAsync(PatchPlayerDTO req, CancellationToken ct)
    {
        var id = Route<Guid>("Id");
        var result = await _mediator.Send(new GetPlayerQuery(id), ct);

        if (result.IsFailure)
        {
            return result.Error.ToProblemDetails();
        }

        var player = _mapper.PlayerDtoToUpdatePlayerDto(result.Value);

        try
        {
            req.Patch(player);
        }
        catch (JsonPatchException e)
        {
            return new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest, Detail = $"Failed to apply patch: {e.Message}"
            };
        }

        var updateResult = await _mediator.Send(new UpdatePlayerCommand(id, player), ct);

        return updateResult.IsSuccess
            ? TypedResults.Ok(updateResult.Value)
            : updateResult.Error.ToProblemDetails();
    }
}