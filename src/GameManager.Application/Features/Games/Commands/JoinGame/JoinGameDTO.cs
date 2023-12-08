using System.ComponentModel.DataAnnotations;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameDTO
{
    [Required]
    public string EntryCode { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
}