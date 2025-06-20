using FastEndpoints;

namespace GameManager.Server.Endpoints;

public class GamesGroup : Group
{
    public GamesGroup()
    {
        Configure("Games", ep =>
        {
            ep.Description(x => x
                .WithTags("Games"));
        });
    }
}