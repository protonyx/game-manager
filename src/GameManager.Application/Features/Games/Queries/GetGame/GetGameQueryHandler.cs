using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetGame;

public class GetGameQueryHandler : IQueryHandler<GetGameQuery, GetGameQueryResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IMapper _mapper;

    public GetGameQueryHandler(IGameRepository gameRepository, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetGameQueryResponse, ApplicationError>> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(request.GameId);
        }

        var dto = _mapper.Map<GameDTO>(game);

        return new GetGameQueryResponse(dto, game.ETag);
    }
}