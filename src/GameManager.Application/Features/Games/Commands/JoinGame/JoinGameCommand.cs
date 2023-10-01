using System.ComponentModel.DataAnnotations;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommand : IRequest<Result<PlayerCredentialsDTO, CommandError>>
{
    public string EntryCode { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
}