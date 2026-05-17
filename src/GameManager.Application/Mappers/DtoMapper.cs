using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetGameSummary;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace GameManager.Application.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DtoMapper
{
    [MapProperty("Name.Value", "Name")]
    [MapProperty("EntryCode.Value", "EntryCode")]
    [MapProperty("CurrentTurn.StartTime", "LastTurnStartTime")]
    public partial GameDTO GameToDto(Game game);

    public partial GameOptionsDTO GameOptionsToDto(GameOptions options);
    public partial GameOptions DtoToGameOptions(GameOptionsDTO dto);

    [MapProperty("Name.Value", "Name")]
    [MapperIgnoreTarget("State")]
    [MapperIgnoreTarget("TrackerValues")]
    private partial PlayerDTO PlayerToDtoGenerated(Player player);

    public PlayerDTO PlayerToDto(Player player)
    {
        var dto = PlayerToDtoGenerated(player);
        dto.State = player.Connections.Count > 0 ? PlayerState.Connected : PlayerState.Disconnected;
        dto.TrackerValues = player.TrackerValues.ToDictionary(tv => tv.TrackerId, tv => tv.Value);
        return dto;
    }

    public partial UpdatePlayerDTO PlayerDtoToUpdatePlayerDto(PlayerDTO dto);

    [MapProperty("Name.Value", "Name")]
    [MapperIgnoreTarget("Turns")]
    [MapperIgnoreTarget("TrackerHistory")]
    public partial PlayerSummaryDTO PlayerToSummaryDto(Player player);

    public partial TrackerDTO TrackerToDto(Tracker tracker);

    [MapperIgnoreTarget("SecondsSinceGameStart")]
    public partial TrackerHistoryDTO TrackerHistoryToDto(TrackerHistory history);

    [MapProperty(nameof(Turn.Duration), nameof(TurnDTO.DurationSeconds))]
    public partial TurnDTO TurnToDto(Turn turn);

    private string MapGameName(GameName name) => name.Value;
    private string MapPlayerName(PlayerName name) => name.Value;
    private string MapTrackerName(TrackerName name) => name.Value;
    private string? MapEntryCode(EntryCode? code) => code?.Value;
    private int MapTimeSpan(TimeSpan ts) => (int)ts.TotalSeconds;
}
