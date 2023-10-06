using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Result<GameDTO, ApplicationError>>
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

    public async Task<Result<GameDTO, ApplicationError>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var options = _mapper.Map<GameOptions>(request.Options) ?? new GameOptions();
        var game = new Game(request.Name, options);

        foreach (var trackerDto in request.Trackers)
        {
            var tracker = _mapper.Map<Tracker>(trackerDto);
            game.AddTracker(tracker);
        }

        while (await _gameRepository.EntryCodeExistsAsync(game.EntryCode, cancellationToken))
        {
            // Generate a new entry code until we find a unique code
            game.RegenerateEntryCode();
        }
        
        // Validate
        var validationResult = await _gameValidator.ValidateAsync(game, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ApplicationError.Validation<Game>(validationResult);
        }

        game = await _gameRepository.CreateAsync(game, cancellationToken);

        var dto = _mapper.Map<GameDTO>(game);

        return dto;
    }
}