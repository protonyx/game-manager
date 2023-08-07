using AutoMapper;
using GameManager.Application.Commands;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Contracts.Queries;
using GameManager.Application.Features.Games.DTO;
using GameManager.Application.Queries;
using GameManager.Domain.Common;
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
            Players = players.Select(_mapper.Map<PlayerSummaryDTO>).ToList()
        };

        return QueryResponses.Object(ret);
    }
}