﻿using System.ComponentModel.DataAnnotations;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommand : IRequest<Result<PlayerCredentialsDTO, ApplicationError>>
{
    public string EntryCode { get; }

    public string Name { get; }

    public JoinGameCommand(string entryCode, string name)
    {
        EntryCode = entryCode;
        Name = name;
    }
}