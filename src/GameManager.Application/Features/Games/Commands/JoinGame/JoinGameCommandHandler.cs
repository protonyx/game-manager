using FluentValidation;
using GameManager.Application.Authorization;
using GameManager.Application.Commands;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerCreated;
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

    private readonly IMediator _mediator;

    public JoinGameCommandHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        ITokenService tokenService,
        IValidator<Player> playerValidator,
        IMediator mediator)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _tokenService = tokenService;
        _playerValidator = playerValidator;
        _mediator = mediator;
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
        
        await _mediator.Publish(new PlayerCreatedNotification(newPlayer), cancellationToken);

        // Generate player token
        var identityBuilder = new PlayerIdentityBuilder();
        identityBuilder.AddGameId(game.Id)
            .AddPlayerId(newPlayer.Id);
        if (newPlayer.IsAdmin)
        {
            identityBuilder.AddAdminRole();
        }
        var token = _tokenService.GenerateToken(identityBuilder.Build());

        var dto = new PlayerCredentialsDTO
        {
            GameId = game.Id,
            PlayerId = newPlayer.Id,
            Token = token,
            IsAdmin = newPlayer.IsAdmin
        };

        return CommandResponses.Data(game.Id, dto);
    }
}