using FastEndpoints;

namespace GameManager.Server.Endpoints.Push;

public class PushGroup : Group
{
    public PushGroup()
    {
        Configure("Push", endpoint => endpoint.Description(description => description.WithTags("Push")));
    }
}
