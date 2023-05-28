using MediatR;

namespace GameManager.Application.Features.Games.Commands;

public class JoinGameCommand : IRequest<JoinGameResponse>
{
    public string EntryCode { get; set; }

    public string Name { get; set; }
}