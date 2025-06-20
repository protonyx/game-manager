using AutoMapper;
using GameManager.Application.Features.Games.Commands.UpdatePlayer;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Features.Games.Queries.GetGameSummary;
using GameManager.Domain.Entities;

namespace GameManager.Application.Profiles;

public class DtoProfile : Profile
{
    public DtoProfile()
    {
        CreateMap<Game, GameDTO>()
            .ForMember(t => t.Name, opt => opt.MapFrom(t => t.Name.Value))
            .ForMember(t => t.EntryCode, opt => opt.MapFrom(t => t.EntryCode.Value))
            .ForMember(t => t.LastTurnStartTime, opt => opt.MapFrom(t => t.CurrentTurn.StartTime))
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.EntryCode, opt => opt.Ignore());
        CreateMap<GameOptions, GameOptionsDTO>()
            .ReverseMap();
        CreateMap<Player, PlayerDTO>()
            .ForMember(t => t.Name, opt => opt.MapFrom(t => t.Name.Value))
            .ForMember(t => t.State, opt => opt.MapFrom(t => t.Connections.Count > 0 ? PlayerState.Connected : PlayerState.Disconnected))
            .ForMember(t => t.TrackerValues, opt =>
                opt.MapFrom(t => t.TrackerValues.ToDictionary(tv => tv.TrackerId, tv => tv.Value)));
        CreateMap<PlayerDTO, UpdatePlayerDTO>();
        CreateMap<Player, PlayerSummaryDTO>()
            .ForMember(t => t.Name, opt => opt.MapFrom(t => t.Name.Value))
            .ForMember(t => t.Turns, opt => opt.Ignore())
            .ForMember(t => t.TrackerHistory, opt => opt.Ignore());
        CreateMap<Tracker, TrackerDTO>();
        CreateMap<TrackerHistory, TrackerHistoryDTO>()
            .ForMember(t => t.SecondsSinceGameStart, opt => opt.Ignore());
        CreateMap<Turn, TurnDTO>()
            .ForMember(t => t.DurationSeconds, opt => opt.MapFrom(t => t.Duration.TotalSeconds));
    }
}