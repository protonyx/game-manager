using GameManager.Application.Contracts.Commands;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Result<GameDTO, CommandError>>
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

    public async Task<Result<GameDTO, CommandError>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var game = new Game()
        {
            Name = request.Name,
            EntryCode = EntryCode.New(EntryCodeLength),
            Options = _mapper.Map<GameOptions>(request.Options) ?? new GameOptions(),
            Trackers = request.Trackers.Select(_mapper.Map<Tracker>).ToList()
        };

        while (await _gameRepository.EntryCodeExistsAsync(game.EntryCode.Value))
        {
            // Generate a new entry code until we find a unique code
            game.EntryCode = EntryCode.New(EntryCodeLength);
        }
        
        // Validate
        var validationResult = await _gameValidator.ValidateAsync(game, cancellationToken);

        if (!validationResult.IsValid)
        {
            return CommandError.Validation<Game>(validationResult);
        }

        game = await _gameRepository.CreateAsync(game);

        var dto = _mapper.Map<GameDTO>(game);

        return dto;
    }
}