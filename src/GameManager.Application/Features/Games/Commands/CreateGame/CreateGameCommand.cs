using GameManager.Application.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommand : IRequest<CreateGameCommandResponse>
{
    public string Name { get; set; } = String.Empty;
    
    public GameOptionsDTO? Options { get; set; }

    public ICollection<TrackerDTO> Trackers { get; set; } = new List<TrackerDTO>();
}