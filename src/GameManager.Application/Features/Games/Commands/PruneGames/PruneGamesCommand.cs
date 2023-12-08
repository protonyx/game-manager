using GameManager.Application.Errors;

namespace GameManager.Application.Features.Games.Commands.PruneGames;

public class PruneGamesCommand : IRequest<UnitResult<ApplicationError>>
{
    public TimeSpan RetentionPeriod { get; }

    public PruneGamesCommand(TimeSpan retentionPeriod)
    {
        RetentionPeriod = retentionPeriod;
    }
}