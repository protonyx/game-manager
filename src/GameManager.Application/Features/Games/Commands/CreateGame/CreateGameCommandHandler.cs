using AutoMapper;
using FluentValidation;
using GameManager.Application.Data;
using GameManager.Application.DTO;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, CreateGameCommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IValidator<Game> _gameValidator;

    private readonly IMapper _mapper;

    public CreateGameCommandHandler(IGameRepository gameRepository, IValidator<Game> gameValidator, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _gameValidator = gameValidator;
        _mapper = mapper;
    }

    public async Task<CreateGameCommandResponse> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var ret = new CreateGameCommandResponse();
        
        var game = new Game()
        {
            Name = request.Name,
            Options = _mapper.Map<GameOptions>(request.Options) ?? new GameOptions(),
            Trackers = request.Trackers.Select(_mapper.Map<Tracker>).ToList()
        };
        
        // Validate
        ret.ValidationResult = await _gameValidator.ValidateAsync(game, cancellationToken);

        if (!ret.ValidationResult.IsValid)
        {
            return ret;
        }

        game = await _gameRepository.CreateAsync(game);

        ret.Game = _mapper.Map<GameDTO>(game);

        return ret;
    }
}