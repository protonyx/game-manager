using System.ComponentModel.DataAnnotations;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameDTO
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = String.Empty;
    
    public GameOptionsDTO? Options { get; set; }

    public ICollection<TrackerDTO> Trackers { get; set; } = new List<TrackerDTO>();
}