using System.ComponentModel.DataAnnotations;
using GameManager.Application.Commands;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommand : IRequest<ICommandResponse>
{
    public string EntryCode { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
}