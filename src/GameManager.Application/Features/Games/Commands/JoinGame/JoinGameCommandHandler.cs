using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Notifications.PlayerCreated;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.JoinGame;

public class JoinGameCommandHandler : IRequestHandler<JoinGameCommand, Result<PlayerCredentialsDTO, ApplicationError>>
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

    public async Task<Result<PlayerCredentialsDTO, ApplicationError>> Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var entryCodeOrError = EntryCode.From(request.EntryCode);

        if (entryCodeOrError.IsFailure)
            return GameErrors.InvalidEntryCode();

        var game = await _gameRepository.GetGameByEntryCodeAsync(entryCodeOrError.Value, cancellationToken);

        if (game == null)
            return GameErrors.InvalidEntryCode();

        // Generate token
        var identityBuilder = new PlayerIdentityBuilder();
        identityBuilder.AddGameId(game.Id);

        var dto = new PlayerCredentialsDTO
        {
            GameId = game.Id,
        };

        if (!request.Observer)
        {
            var playerNameOrError = PlayerName.From(request.Name);

            if (playerNameOrError.IsFailure)
                return GameErrors.PlayerInvalidName(playerNameOrError.Error);

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
                return ApplicationError.Validation<Player>(validationResult);
            }

            newPlayer = await _playerRepository.CreateAsync(newPlayer, cancellationToken);

            await _mediator.Publish(new PlayerCreatedNotification(newPlayer), cancellationToken);

            identityBuilder.AddPlayerId(newPlayer.Id);

            if (newPlayer.IsHost)
            {
                identityBuilder.AddHostRole();
            }

            dto.PlayerId = newPlayer.Id;
            dto.IsHost = newPlayer.IsHost;
        }

        dto.Token = _tokenService.GenerateToken(identityBuilder.Build());

        return dto;
    }
}