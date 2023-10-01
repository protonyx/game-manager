using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.PruneGames;

public class PruneGamesCommandHandler : IRequestHandler<PruneGamesCommand, UnitResult<CommandError>>
{
    private readonly IGameRepository _gameRepository;

    public PruneGamesCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<UnitResult<CommandError>> Handle(PruneGamesCommand request, CancellationToken cancellationToken)
    {
        var olderThan = DateTime.UtcNow.Subtract(request.RetentionPeriod);

        var games = await _gameRepository.FindAsync(olderThan);

        foreach (var game in games)
        {
            await _gameRepository.DeleteAsync(game);
        }

        return UnitResult.Success<CommandError>();
    }
}