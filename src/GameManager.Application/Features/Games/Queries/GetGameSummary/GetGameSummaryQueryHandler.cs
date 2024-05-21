using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Queries.GetGameSummary;

public class GetGameSummaryQueryHandler : IRequestHandler<GetGameSummaryQuery, Result<GameSummaryDTO, ApplicationError>>
{
    private readonly IGameRepository _gameRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly ITurnRepository _turnRepository;

    private readonly ITrackerHistoryRepository _trackerHistoryRepository;

    private readonly IMapper _mapper;

    public GetGameSummaryQueryHandler(
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        ITurnRepository turnRepository,
        ITrackerHistoryRepository trackerHistoryRepository,
        IMapper mapper)
    {
        _gameRepository = gameRepository;
        _playerRepository = playerRepository;
        _turnRepository = turnRepository;
        _trackerHistoryRepository = trackerHistoryRepository;
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

        var players = await _playerRepository.GetPlayersByGameIdAsync(game.Id, cancellationToken);
        var turns = await _turnRepository.GetTurnsByGameId(request.GameId, cancellationToken);
        var trackerHistory = await _trackerHistoryRepository.GetHistoryByGameId(request.GameId, cancellationToken);

        var ret = new GameSummaryDTO()
        {
            Id = game.Id,
            Name = game.Name,
            CreatedDate = game.CreatedDate,
            StartedDate = game.StartedDate,
            CompletedDate = game.CompletedDate,
            Trackers = game.Trackers.Select(_mapper.Map<TrackerDTO>).ToList(),
            Players = players.Select(p =>
            {
                var dto = _mapper.Map<PlayerSummaryDTO>(p);

                dto.Turns = turns.Where(t => t.PlayerId == p.Id)
                    .Select(_mapper.Map<TurnDTO>)
                    .ToList();
                dto.TrackerHistory = trackerHistory.Where(t => t.PlayerId == p.Id)
                    .Select(t =>
                    {
                        var thDto = _mapper.Map<TrackerHistoryDTO>(t);
                        var timeDiff = thDto.ChangedTime - (game.StartedDate ?? game.CreatedDate);
                        thDto.SecondsSinceGameStart = (int)Math.Abs(timeDiff.TotalSeconds);

                        return thDto;
                    })
                    .ToList();

                return dto;
            }).ToList()
        };

        return ret;
    }
}