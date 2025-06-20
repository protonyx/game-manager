using GameManager.Server.DTO;

namespace GameManager.Server.Endpoints.Players;

public class PatchPlayerDTO : PatchRequest
{
    public Guid Id { get; set; }
}