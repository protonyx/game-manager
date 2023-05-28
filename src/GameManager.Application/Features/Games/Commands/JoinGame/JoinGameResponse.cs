using System.ComponentModel.DataAnnotations;
using GameManager.Server.DTO;

namespace GameManager.Application.Features.Games.Commands;

public class JoinGameResponse
{
    public ICollection<ValidationResult> ValidationResults { get; set; } = new List<ValidationResult>();

    public PlayerCredentialsDTO? Credentials { get; set; }
}