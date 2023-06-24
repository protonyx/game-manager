using MediatR;

namespace GameManager.Application.Features.Games.Commands.PruneGames;

public class PruneGamesCommand : IRequest
{
    public TimeSpan RetentionPeriod { get; }

    public PruneGamesCommand(TimeSpan retentionPeriod)
    {
        RetentionPeriod = retentionPeriod;
    }
}