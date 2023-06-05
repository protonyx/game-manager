using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using GameManager.Application.Data;
using GameManager.Application.Services;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommandHandler : IRequestHandler<JoinGameCommand, JoinGameCommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly ITokenService _tokenService;

    private readonly IValidator<Player> _playerValidator;

    public JoinGameCommandHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        ITokenService tokenService,
        IValidator<Player> playerValidator)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _tokenService = tokenService;
        _playerValidator = playerValidator;
    }

    public async Task<JoinGameCommandResponse> Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var ret = new JoinGameCommandResponse();
        
        var game = await _gameRepository.GetGameByEntryCodeAsync(request.EntryCode);
        
        if (game == null)
        {
            ret.ValidationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new ValidationFailure(nameof(JoinGameCommand.EntryCode), "The entry code is invalid.")
            });

            return ret;
        }

        var newPlayer = new Player()
        {
            GameId = game.Id,
            Name = request.Name
        };
        
        // Validate
        ret.ValidationResult = await _playerValidator.ValidateAsync(newPlayer, cancellationToken);

        if (!ret.ValidationResult.IsValid)
        {
            return ret;
        }

        newPlayer = await _playerRepository.CreateAsync(newPlayer);
        
        ret.GameId = game.Id;
        ret.PlayerId = newPlayer.Id;
        ret.Token = _tokenService.GenerateToken(game.Id, newPlayer.Id, newPlayer.IsAdmin);
        ret.IsAdmin = newPlayer.IsAdmin;

        return ret;
    }
}