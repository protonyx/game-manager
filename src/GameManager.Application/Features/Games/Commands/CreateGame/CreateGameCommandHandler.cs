using AutoMapper;
using GameManager.Application.Data;
using GameManager.Application.DTO;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, CreateGameCommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IMapper _mapper;

    public CreateGameCommandHandler(IGameRepository gameRepository, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
    }

    public async Task<CreateGameCommandResponse> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = new Game()
        {
            Name = request.Name,
            Options = _mapper.Map<GameOptions>(request.Options) ?? new GameOptions(),
            Trackers = request.Trackers.Select(_mapper.Map<Tracker>).ToList()
        };
        
        // TODO: Validate game

        game = await _gameRepository.CreateAsync(game);

        var dto = _mapper.Map<GameDTO>(game);

        return new CreateGameCommandResponse()
        {
            Game = dto
        };
    }
}