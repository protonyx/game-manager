using AutoMapper;
using FluentValidation;
using GameManager.Application.Commands;
using GameManager.Application.Data;
using GameManager.Application.DTO;
using GameManager.Application.Services;
using GameManager.Domain.Entities;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, ICommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IValidator<Game> _gameValidator;

    private readonly IMapper _mapper;
    
    private const int EntryCodeLength = 4;

    public CreateGameCommandHandler(IGameRepository gameRepository, IValidator<Game> gameValidator, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _gameValidator = gameValidator;
        _mapper = mapper;
    }

    public async Task<ICommandResponse> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = new Game()
        {
            Name = request.Name,
            EntryCode = EntryCodeFactory.Create(EntryCodeLength),
            Options = _mapper.Map<GameOptions>(request.Options) ?? new GameOptions(),
            Trackers = request.Trackers.Select(_mapper.Map<Tracker>).ToList()
        };

        while (await _gameRepository.EntryCodeExistsAsync(game.EntryCode))
        {
            // Generate a new entry code until we find a unique code
            game.EntryCode = EntryCodeFactory.Create(EntryCodeLength);
        }
        
        // Validate
        var validationResult = await _gameValidator.ValidateAsync(game, cancellationToken);

        if (!validationResult.IsValid)
        {
            return CommandResponses.ValidationError(validationResult);
        }

        game = await _gameRepository.CreateAsync(game);

        var dto = _mapper.Map<GameDTO>(game);

        return CommandResponses.Data(game.Id, dto);
    }
}