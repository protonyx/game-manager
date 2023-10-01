using GameManager.Application.Contracts.Commands;

namespace GameManager.Application.Features.Games.Commands.PruneGames;

public class PruneGamesCommand : IRequest<UnitResult<CommandError>>
{
    public TimeSpan RetentionPeriod { get; }

    public PruneGamesCommand(TimeSpan retentionPeriod)
    {
        RetentionPeriod = retentionPeriod;
    }
}