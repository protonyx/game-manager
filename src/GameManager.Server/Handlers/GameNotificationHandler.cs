using AutoMapper;
using GameManager.Server.DTO;
using GameManager.Server.Messages;
using GameManager.Server.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server.Handlers;

public class GameNotificationHandler :
    INotificationHandler<GameUpdatedNotification>
{
    private readonly IHubContext<GameHub> _hubContext;

    private readonly IMapper _mapper;

    public GameNotificationHandler(IHubContext<GameHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task Handle(GameUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var game = notification.Game;
        var dto = _mapper.Map<GameDTO>(game);
        
        var message = new GameStateChangedMessage()
        {
            GameId = game.Id,
            Game = dto
        };
        
        await _hubContext.Clients.Group(game.Id.ToString())
            .SendAsync(nameof(IGameHubClient.GameStateChanged),
                message,
                cancellationToken: cancellationToken);
    }
}