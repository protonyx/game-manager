using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerCreated;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommandHandler : IRequestHandler<JoinGameCommand, Result<PlayerCredentialsDTO, CommandError>>
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

    public async Task<Result<PlayerCredentialsDTO, CommandError>> Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var entryCodeOrError = EntryCode.From(request.EntryCode);

        if (entryCodeOrError.IsFailure)
            return GameErrors.Commands.InvalidEntryCode();
        
        var playerNameOrError = PlayerName.From(request.Name);

        if (playerNameOrError.IsFailure)
            return GameErrors.Commands.PlayerInvalidName(playerNameOrError.Error);
        
        var game = await _gameRepository.GetGameByEntryCodeAsync(entryCodeOrError.Value, cancellationToken);
        
        if (game == null)
            return GameErrors.Commands.InvalidEntryCode();

        var newPlayer = new Player(playerNameOrError.Value, game);
        
        // Promote the player if they are the first
        var existingPlayerCount = await _playerRepository.GetActivePlayerCountAsync(game.Id, cancellationToken);

        newPlayer.SetOrder(existingPlayerCount + 1);
        
        if (existingPlayerCount == 0)
        {
            newPlayer.Promote();
        }
        
        // Validate
        var validationResult = await _playerValidator.ValidateAsync(newPlayer, cancellationToken);

        if (!validationResult.IsValid)
        {
            return CommandError.Validation<Player>(validationResult);
        }

        newPlayer = await _playerRepository.CreateAsync(newPlayer, cancellationToken);
        
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

        return dto;
    }
}