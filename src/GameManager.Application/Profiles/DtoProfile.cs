using AutoMapper;
using GameManager.Application.DTO;
using GameManager.Domain.Entities;

namespace GameManager.Application.Profiles;

public class DtoProfile : Profile
{
    public DtoProfile()
    {
        CreateMap<Game, GameDTO>()
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.EntryCode, opt => opt.Ignore());
        CreateMap<GameOptions, GameOptionsDTO>()
            .ReverseMap()
            .ForMember(t => t.GameId, opt => opt.Ignore());
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
        CreateMap<Tracker, TrackerDTO>()
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.GameId, opt => opt.Ignore());
    }
}