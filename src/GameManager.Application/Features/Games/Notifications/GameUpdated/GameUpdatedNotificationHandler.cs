using AutoMapper;
using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Messages;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications.GameUpdated;

public class GameUpdatedNotificationHandler : INotificationHandler<GameUpdatedNotification>
{

    private readonly IGameClientNotificationService _gameClientNotificationService;

    private readonly IMapper _mapper;

    public GameUpdatedNotificationHandler(IGameClientNotificationService gameClientNotificationService, IMapper mapper)
    {
        _gameClientNotificationService = gameClientNotificationService;
        _mapper = mapper;
    }

    public Task Handle(GameUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var game = notification.Game;
        var dto = _mapper.Map<GameDTO>(game);

        var message = new GameStateChangedMessage()
        {
            GameId = game.Id,
            Game = dto
        };

        return _gameClientNotificationService.GameStateChanged(message, cancellationToken);
    }
}