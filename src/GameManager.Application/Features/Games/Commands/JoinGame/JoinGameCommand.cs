using MediatR;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommand : IRequest<JoinGameCommandResponse>
{
    public string EntryCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}