using System.ComponentModel.DataAnnotations;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommand : IRequest<Result<PlayerCredentialsDTO, ApplicationError>>
{
    public string EntryCode { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
}