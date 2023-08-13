using AutoMapper;
using GameManager.Application.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Contracts.Queries;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Queries;
using GameManager.Domain.Common;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetGameSummary;

public class GetGameSummaryQueryHandler : IRequestHandler<GetGameSummaryQuery, IQueryResponse<GameSummaryDTO>>
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

    public async Task<IQueryResponse<GameSummaryDTO>> Handle(GetGameSummaryQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        if (game == null)
        {
            return QueryResponses.NotFound<GameSummaryDTO>();
        }
        else if (game.State != GameState.Complete)
        {
            return QueryResponses.AuthorizationError<GameSummaryDTO>("Summary only available for completed games");
        }

        var players = await _playerRepository.GetSummariesByGameIdAsync(game.Id);

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

                foreach (var trackerHistory in dto.TrackerHistory)
                {
                    var timeDiff = trackerHistory.ChangedTime - (game.StartedDate ?? game.CreatedDate);
                    trackerHistory.SecondsSinceGameStart = (int)Math.Abs(timeDiff.TotalSeconds);
                }
                
                return dto;
            }).ToList()
        };

        return QueryResponses.Object(ret);
    }
}