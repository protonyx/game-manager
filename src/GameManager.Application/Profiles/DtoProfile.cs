using AutoMapper;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.Entities;

namespace GameManager.Application.Profiles;

public class DtoProfile : Profile
{
    public DtoProfile()
    {
        CreateMap<Game, GameDTO>()
            .ForMember(t => t.LastTurnStartTime, opt => opt.MapFrom(t => t.CurrentTurn.StartTime))
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.EntryCode, opt => opt.Ignore());
        CreateMap<GameOptions, GameOptionsDTO>()
            .ReverseMap();
        CreateMap<Player, PlayerDTO>()
            .ForMember(t => t.TrackerValues, opt =>
                opt.MapFrom(t => t.TrackerValues.ToDictionary(tv => tv.TrackerId, tv => tv.Value)))
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.TrackerValues, opt =>
                opt.MapFrom(t => t.TrackerValues
                    .Select(kv => new TrackerValue()
                    {
                        TrackerId = kv.Key,
                        Value = kv.Value
                    })
                    .ToList()));
        CreateMap<Player, PlayerSummaryDTO>();
        CreateMap<Tracker, TrackerDTO>()
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.GameId, opt => opt.Ignore());
        CreateMap<TrackerHistory, TrackerHistoryDTO>()
            .ForMember(t => t.SecondsSinceGameStart, opt => opt.Ignore());
        CreateMap<Turn, TurnDTO>()
            .ForMember(t => t.DurationSeconds, opt => opt.MapFrom(t => t.Duration.TotalSeconds));
    }
}