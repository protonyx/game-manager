using GameManager.Application.Contracts;
using GameManager.Application.Errors;
using GameManager.Application.Features.Games.DTO;
using GameManager.Domain.ValueObjects;

namespace GameManager.Application.Features.Games.Commands.CreateGame;

public class CreateGameCommandHandler : ICommandHandler<CreateGameCommand, CreateGameCommandResponse>
{
    private readonly IGameRepository _gameRepository;

    private readonly IMapper _mapper;

    public CreateGameCommandHandler(IGameRepository gameRepository, IMapper mapper)
    {
        _gameRepository = gameRepository;
        _mapper = mapper;
    }

    public async Task<Result<CreateGameCommandResponse, ApplicationError>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var options = _mapper.Map<GameOptions>(request.Options) ?? new GameOptions();

        var gameNameOrError = GameName.From(request.Name);

        if (gameNameOrError.IsFailure)
            return GameErrors.GameInvalidName(gameNameOrError.Error);
        
        var game = new Game(gameNameOrError.Value, options);

        foreach (var tracker in request.Trackers)
        {
            var trackerAddResult = game.AddTracker(tracker.Name, tracker.StartingValue);

            if (trackerAddResult.IsFailure)
                return GameErrors.GameInvalidTracker(trackerAddResult.Error);
        }

        while (await _gameRepository.EntryCodeExistsAsync(game.EntryCode!, cancellationToken))
        {
            // Generate a new entry code until we find a unique code
            var result = game.RegenerateEntryCode();

            if (result.IsFailure)
                return ApplicationError.Failure(result.Error);
        }

        game = await _gameRepository.CreateAsync(game, cancellationToken);

        var dto = _mapper.Map<GameDTO>(game);

        return new CreateGameCommandResponse(dto, game.ETag);
    }
}