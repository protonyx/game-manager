using System.ComponentModel.DataAnnotations;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommand : ICommand<PlayerCredentialsDTO>
{
    public string EntryCode { get; }

    public string Name { get; }

    public bool Observer { get; }

    public string? Color { get; }

    public JoinGameCommand(string entryCode, string name, bool observer = false, string? color = null)
    {
        EntryCode = entryCode;
        Name = name;
        Observer = observer;
        Color = color;
    }
}