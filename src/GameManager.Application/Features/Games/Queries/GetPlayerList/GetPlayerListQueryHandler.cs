using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetPlayerList;

public class GetPlayerListQueryHandler : IQueryHandler<GetPlayerListQuery, IReadOnlyList<PlayerDTO>>
{
    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    public GetPlayerListQueryHandler(IPlayerRepository playerRepository, IMapper mapper)
    {
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<PlayerDTO>, ApplicationError>> Handle(GetPlayerListQuery request, CancellationToken cancellationToken)
    {
        var players = await _playerRepository.GetPlayersByGameIdAsync(request.GameId, cancellationToken);

        var ret = players.Select(_mapper.Map<PlayerDTO>).ToList();

        return ret;
    }
}