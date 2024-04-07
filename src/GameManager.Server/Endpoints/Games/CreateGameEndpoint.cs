using FastEndpoints;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Application.Features.Games.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameManager.Server.Endpoints;

public class CreateGameEndpoint : Endpoint<CreateGameDTO, Results<CreatedAtRoute<GameDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public CreateGameEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("");
        AllowAnonymous();
        Group<GamesGroup>();
    }

    public override async Task<Results<CreatedAtRoute<GameDTO>, ProblemDetails>> ExecuteAsync(
        CreateGameDTO req,
        CancellationToken ct)
    {
        var request = new CreateGameCommand(req);
        
        var result = await _mediator.Send(request, ct);

        return result.IsSuccess
            ? TypedResults.CreatedAtRoute(result.Value, nameof(GetGameEndpoint), new {id = result.Value.Id})
            : result.Error.ToProblemDetails();
    }
}