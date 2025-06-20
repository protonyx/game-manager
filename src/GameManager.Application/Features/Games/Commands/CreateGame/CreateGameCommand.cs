using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommand : ICommand<CreateGameCommandResponse>
{
    public string Name { get; set; } = string.Empty;

    public GameOptionsDTO? Options { get; set; }

    public ICollection<CreateTrackerDTO> Trackers { get; set; } = new List<CreateTrackerDTO>();
}

public class CreateTrackerDTO
{
    public string Name { get; set; } = string.Empty;

    public int StartingValue { get; set; }
}