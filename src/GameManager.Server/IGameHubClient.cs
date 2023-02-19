using GameManager.Server.Messages;

namespace GameManager.Server;

public interface IGameHubClient
{
    Task PlayerJoined(PlayerJoinedMessage message);

    Task GameStateChanged(GameStateChangedMessage message);
}