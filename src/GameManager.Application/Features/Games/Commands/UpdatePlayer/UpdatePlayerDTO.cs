namespace GameManager.Application.Features.Games.Commands.UpdatePlayer
{
    public class UpdatePlayerDTO
    {
        public string Name { get; set; } = string.Empty;

        public IDictionary<Guid, int> TrackerValues { get; set; } = new Dictionary<Guid, int>();
    }
}