using FluentValidation;
using GameManager.Application.Commands;
using GameManager.Application.Data;
using GameManager.Application.DTO;
using GameManager.Application.Services;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommandHandler : IRequestHandler<JoinGameCommand, ICommandResponse>
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

    public async Task<ICommandResponse> Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetGameByEntryCodeAsync(request.EntryCode);
        
        if (game == null)
        {
            return CommandResponses.Failure("The entry code is invalid.");
        }

        var newPlayer = new Player()
        {
            GameId = game.Id,
            Name = request.Name
        };
        
        // Validate
        var validationResult = await _playerValidator.ValidateAsync(newPlayer, cancellationToken);

        if (!validationResult.IsValid)
        {
            return CommandResponses.ValidationError(validationResult);
        }

        newPlayer = await _playerRepository.CreateAsync(newPlayer);

        var dto = new PlayerCredentialsDTO
        {
            GameId = game.Id,
            PlayerId = newPlayer.Id,
            Token = _tokenService.GenerateToken(game.Id, newPlayer.Id, newPlayer.IsAdmin),
            IsAdmin = newPlayer.IsAdmin
        };

        return CommandResponses.Data(game.Id, dto);
    }
}