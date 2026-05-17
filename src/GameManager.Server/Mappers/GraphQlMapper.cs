using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using GameManager.Server.Models;
using Riok.Mapperly.Abstractions;

namespace GameManager.Server.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GraphQlMapper
{
    [MapProperty("Name.Value", "Name")]
    [MapProperty("EntryCode.Value", "EntryCode")]
    [MapperIgnoreTarget("CurrentTurnPlayer")]
    [MapperIgnoreTarget("Players")]
    [MapperIgnoreTarget("Turns")]
    public partial GameModel GameToModel(Game game);

    public partial IQueryable<GameModel> ProjectToGameModel(IQueryable<Game> query);

    public partial GameTrackerModel TrackerToModel(Tracker tracker);

    public partial TurnModel TurnToModel(Turn turn);

    [MapProperty("Name.Value", "Name")]
    [MapProperty("TrackerValues", "Trackers")]
    [MapperIgnoreTarget("State")]
    public partial PlayerModel PlayerToModel(Player player);

    [MapperIgnoreTarget("Name")]
    public partial PlayerTrackerValueModel TrackerValueToModel(TrackerValue trackerValue);

    private string MapGameName(GameName name) => name.Value;
    private string? MapEntryCode(EntryCode? code) => code?.Value;
    private string MapPlayerName(PlayerName name) => name.Value;
    private string MapTrackerName(TrackerName name) => name.Value;
}
