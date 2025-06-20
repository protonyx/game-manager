using AutoMapper;
using GameManager.Domain.Entities;
using GameManager.Server.Models;

namespace GameManager.Server;

public class GraphQlProfile : Profile
{
    public GraphQlProfile()
    {
        CreateMap<Game, GameModel>()
            .ForMember(t => t.Name, opt => opt.MapFrom(t => t.Name.Value))
            .ForMember(t => t.EntryCode, opt => opt.MapFrom(t => t.EntryCode.Value))
            .ForMember(t => t.CurrentTurnPlayer, opt => opt.Ignore())
            .ForMember(t => t.Players, opt => opt.Ignore())
            .ForMember(t => t.Turns, opt => opt.Ignore());
        CreateMap<Tracker, GameTrackerModel>();
        CreateMap<Turn, TurnModel>();

        CreateMap<Player, PlayerModel>()
            .ForMember(t => t.Name, opt => opt.MapFrom(t => t.Name.Value))
            .ForMember(t => t.Trackers, opt => opt.MapFrom(t => t.TrackerValues));
        CreateMap<TrackerValue, PlayerTrackerValueModel>()
            .ForMember(t => t.Name, opt => opt.Ignore());
    }
}