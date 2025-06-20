using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetPlayerTurns;

public class GetPlayerTurnsQueryHandler : IQueryHandler<GetPlayerTurnsQuery, IReadOnlyList<TurnDTO>>
{
    private readonly ITurnRepository _turnRepository;

    private readonly IMapper _mapper;

    public GetPlayerTurnsQueryHandler(ITurnRepository turnRepository, IMapper mapper)
    {
        _turnRepository = turnRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<TurnDTO>, ApplicationError>> Handle(GetPlayerTurnsQuery request, CancellationToken cancellationToken)
    {
        var turns = await _turnRepository.GetTurnsByPlayerId(request.PlayerId, cancellationToken);

        return turns.Select(_mapper.Map<TurnDTO>).ToList();
    }
}