namespace GameManager.Application.Features.Games.Commands.ReorderPlayers;

public class PlayerIdListDTO
{
    public ICollection<Guid> PlayerIds { get; set; } = new List<Guid>();
}