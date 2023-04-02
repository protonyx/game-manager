using GameManager.Server.Models;
using MediatR;

namespace GameManager.Server.Notifications;

public class GameUpdatedNotification : INotification
{
    public Game Game { get; }

    public GameUpdatedNotification(Game game)
    {
        Game = game;
    }
}