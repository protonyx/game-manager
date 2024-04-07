using FastEndpoints;

namespace GameManager.Server.Endpoints;

public class PlayersGroup : Group
{
    public PlayersGroup()
    {
        Configure("Players", ep =>
        {
            ep.Description(x => x
                .WithTags("Players"));
        });
    }
}