using GameManager.Application.Data;
using MediatR;

namespace GameManager.Application.Features.Games.Commands.PruneGames;

public class PruneGamesCommandHandler : IRequestHandler<PruneGamesCommand>
{
    private readonly IGameRepository _gameRepository;

    public PruneGamesCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task Handle(PruneGamesCommand request, CancellationToken cancellationToken)
    {
        var olderThan = DateTime.Now.Subtract(request.RetentionPeriod);

        var games = await _gameRepository.FindAsync(olderThan);

        foreach (var game in games)
        {
            await _gameRepository.DeleteAsync(game);
        }
    }
}