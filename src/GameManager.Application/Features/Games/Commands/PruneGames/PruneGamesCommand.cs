using GameManager.Application.Contracts;

namespace GameManager.Application.Features.Games.Commands.PruneGames;

public class PruneGamesCommand : ICommand
{
    public TimeSpan RetentionPeriod { get; }

    public PruneGamesCommand(TimeSpan retentionPeriod)
    {
        RetentionPeriod = retentionPeriod;
    }
}