using AutoMapper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayerList;

public class GetPlayerListQueryHandler : IRequestHandler<GetPlayerListQuery, ICollection<PlayerDTO>>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    public GetPlayerListQueryHandler(IPlayerRepository playerRepository, IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<PlayerDTO>> Handle(GetPlayerListQuery request, CancellationToken cancellationToken)
    {
        var players = await _playerRepository.GetPlayersByGameIdAsync(request.GameId);

        var ret = _mapper.Map<ICollection<PlayerDTO>>(players);

        return ret;
    }
}