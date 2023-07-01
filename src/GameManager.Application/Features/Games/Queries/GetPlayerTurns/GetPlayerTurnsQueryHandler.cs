using AutoMapper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetPlayerTurns;

public class GetPlayerTurnsQueryHandler : IRequestHandler<GetPlayerTurnsQuery, ICollection<TurnDTO>>
{
    private readonly ITurnRepository _turnRepository;

    private readonly IMapper _mapper;

    public GetPlayerTurnsQueryHandler(ITurnRepository turnRepository, IMapper mapper)
    {
        _turnRepository = turnRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<TurnDTO>> Handle(GetPlayerTurnsQuery request, CancellationToken cancellationToken)
    {
        var turns = await _turnRepository.GetTurnsByPlayerId(request.PlayerId);

        return turns.Select(_mapper.Map<TurnDTO>).ToList();
    }
}