namespace GameManager.Server.Endpoints.Games;

public class JoinGameDTO
{
    /// <summary>
    /// Game Entry Code
    /// </summary>
    public string EntryCode { get; set; } = string.Empty;

    /// <summary>
    /// Player Name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Join as an observer instead of a player
    /// </summary>
    public bool Observer { get; set; } = false;
}