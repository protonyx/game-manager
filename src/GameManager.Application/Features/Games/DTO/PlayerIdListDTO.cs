namespace GameManager.Application.Features.Games.DTO;

public class PlayerIdListDTO
{
    public ICollection<Guid> PlayerIds { get; set; } = new List<Guid>();
}