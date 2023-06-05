using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GameManager.Application.DTO;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandResponse : ValidateCommandResponseBase
{
    public GameDTO? Game { get; set; }
}