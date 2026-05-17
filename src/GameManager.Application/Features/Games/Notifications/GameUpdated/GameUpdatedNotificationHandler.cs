using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Mappers;
using GameManager.Application.Messages;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.GameUpdated;

public class GameUpdatedNotificationHandler : INotificationHandler<GameUpdatedNotification>
{

    private readonly IGameClientNotificationService _gameClientNotificationService;

    private readonly DtoMapper _mapper;

    public GameUpdatedNotificationHandler(IGameClientNotificationService gameClientNotificationService, DtoMapper mapper)
    {
        _gameClientNotificationService = gameClientNotificationService;
        _mapper = mapper;
    }

    public Task Handle(GameUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var game = notification.Game;
        var dto = _mapper.GameToDto(game);

        var message = new GameStateChangedMessage()
        {
            GameId = game.Id,
            Game = dto
        };

        return _gameClientNotificationService.GameStateChanged(message, cancellationToken);
    }
}