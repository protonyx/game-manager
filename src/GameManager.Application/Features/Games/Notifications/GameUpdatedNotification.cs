using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Notifications;

public class GameUpdatedNotification : INotification
{
    public Game Game { get; }

    public GameUpdatedNotification(Game game)
    {
        Game = game;
    }
}