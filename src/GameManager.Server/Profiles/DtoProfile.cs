using AutoMapper;
using GameManager.Server.DTO;
using GameManager.Server.Models;

namespace GameManager.Server.Profiles;

public class DtoProfile : Profile
{
    public DtoProfile()
    {
        CreateMap<NewGameDTO, Game>()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.EntryCode, opt => opt.Ignore())
            .ForMember(t => t.Players, opt => opt.Ignore());

        CreateMap<Game, GameDTO>();
        CreateMap<GameOptions, GameOptionsDTO>()
            .ReverseMap()
            .ForMember(t => t.GameId, opt => opt.Ignore());
        CreateMap<Player, PlayerDTO>()
            .ForMember(t => t.TrackerValues, opt => 
                opt.MapFrom(t => t.TrackerValues.ToDictionary(tv => tv.Id, tv => tv.Value)));
        CreateMap<Player, PlayerCredentialsDTO>()
            .ForMember(t => t.PlayerId, opt => opt.MapFrom(t => t.Id))
            .ForMember(t => t.Token, opt => opt.Ignore());
        CreateMap<Tracker, TrackerDTO>()
            .ReverseMap()
            .ForMember(t => t.Id, opt => opt.Ignore())
            .ForMember(t => t.GameId, opt => opt.Ignore())
            .ForMember(t => t.Game, opt => opt.Ignore());
    }
}