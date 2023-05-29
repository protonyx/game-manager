using AutoMapper;
using GameManager.Application.Data;
using GameManager.Application.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayer;

public class GetPlayerQueryHandler : IRequestHandler<GetPlayerQuery, PlayerDTO?>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    public GetPlayerQueryHandler(IPlayerRepository playerRepository, IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    public async Task<PlayerDTO?> Handle(GetPlayerQuery request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId);

        var dto = _mapper.Map<PlayerDTO>(player);

        return dto;
    }
}