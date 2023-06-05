using AutoMapper;
using GameManager.Application.DTO;
using GameManager.Application.Features.Games.Notifications;
using GameManager.Server.Messages;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace GameManager.Server.Handlers;

public class PlayerNotificationHandler :
    INotificationHandler<PlayerUpdatedNotification>,
    INotificationHandler<PlayerCreatedNotification>,
    INotificationHandler<PlayerDeletedNotification>
{
    private readonly IHubContext<GameHub> _hubContext;

    private readonly IMapper _mapper;

    public PlayerNotificationHandler(IHubContext<GameHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task Handle(PlayerUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;
        var dto = _mapper.Map<PlayerDTO>(player);
        
        var message = new PlayerStateChangedMessage()
        {
            GameId = player.GameId,
            PlayerId = player.Id,
            Player = dto
        };
        
        await _hubContext.Clients.Group(player.GameId.ToString())
            .SendAsync(nameof(IGameHubClient.PlayerStateChanged),
                message,
                cancellationToken: cancellationToken);
    }

    public async Task Handle(PlayerCreatedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;
        var dto = _mapper.Map<PlayerDTO>(player);
        
        var message = new PlayerJoinedMessage()
        {
            GameId = player.GameId,
            Player = dto
        };
        
        await _hubContext.Clients.Group(player.GameId.ToString())
            .SendAsync(nameof(IGameHubClient.PlayerJoined),
                message,
                cancellationToken: cancellationToken);
    }

    public async Task Handle(PlayerDeletedNotification notification, CancellationToken cancellationToken)
    {
        var player = notification.Player;
        
        var message = new PlayerLeftMessage()
        {
            GameId = player.GameId,
            PlayerId = player.Id
        };
        
        await _hubContext.Clients.Group(player.GameId.ToString())
            .SendAsync(nameof(IGameHubClient.PlayerLeft),
                message,
                cancellationToken: cancellationToken);
    }
}