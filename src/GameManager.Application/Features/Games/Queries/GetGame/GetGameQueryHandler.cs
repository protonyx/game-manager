using AutoMapper;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games.DTO;
using MediatR;

namespace GameManager.Application.Features.Games.Queries.GetGame;

public class GetGameQueryHandler : IRequestHandler<GetGameQuery, GameDTO?>
{
    private readonly IGameRepository _gameRepository;

    private readonly IMapper _mapper;

    public GetGameQueryHandler(IGameRepository gameRepository, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
    }

    public async Task<GameDTO?> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.GameId);

        var dto = _mapper.Map<GameDTO>(game);

        return dto;
    }
}