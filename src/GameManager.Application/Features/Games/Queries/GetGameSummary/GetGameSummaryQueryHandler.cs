using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetGameSummary;

public class GetGameSummaryQueryHandler : IRequestHandler<GetGameSummaryQuery, Result<GameSummaryDTO, ApplicationError>>
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly IMapper _mapper;

    public GetGameSummaryQueryHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        IMapper mapper)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _mapper = mapper;
    }

    public async Task<Result<GameSummaryDTO, ApplicationError>> Handle(GetGameSummaryQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);

        if (game == null)
        {
            return GameErrors.GameNotFound(request.GameId);
        }
        else if (game.State != GameState.Complete)
        {
            return GameErrors.GameNotComplete();
        }

        var players = await _playerRepository.GetSummariesByGameIdAsync(game.Id, cancellationToken);

        var ret = new GameSummaryDTO()
        {
            Id = game.Id,
            Name = game.Name,
            CreatedDate = game.CreatedDate,
            CompletedDate = game.CompletedDate,
            Trackers = game.Trackers.Select(_mapper.Map<TrackerDTO>).ToList(),
            Players = players.Select(p =>
            {
                var dto = _mapper.Map<PlayerSummaryDTO>(p);
                
                // TODO: Get Turns and TrackerHistory

                foreach (var trackerHistory in dto.TrackerHistory)
                {
                    var timeDiff = trackerHistory.ChangedTime - (game.StartedDate ?? game.CreatedDate);
                    trackerHistory.SecondsSinceGameStart = (int)Math.Abs(timeDiff.TotalSeconds);
                }
                
                return dto;
            }).ToList()
        };

        return ret;
    }
}