using FastEndpoints;
using GameManager.Application.Features.Games.Commands.JoinGame;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Server.Endpoints.Games;

public class JoinGameEndpoint : Endpoint<JoinGameDTO, Results<Ok<PlayerCredentialsDTO>, ProblemDetails>>
{
    private readonly IMediator _mediator;

    public JoinGameEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("Join");
        Group<GamesGroup>();
        AllowAnonymous();
        Version(1);
    }

    public override async Task<Results<Ok<PlayerCredentialsDTO>, ProblemDetails>> ExecuteAsync(JoinGameDTO req, CancellationToken ct)
    {
        var request = new JoinGameCommand(req.EntryCode, req.Name, req.Observer);

        var result = await _mediator.Send(request, ct);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.ToProblemDetails();
    }
}