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
    }
}