using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GameManager.Server.Authentication;

public class CustomJwtBearerEvents : JwtBearerEvents
{
    public override Task MessageReceived(MessageReceivedContext context)
    {
        // SignalR shim for tokens passed via query parameters
        var accessToken = context.Request.Query["access_token"];

        // If the request is for our hub...
        var path = context.HttpContext.Request.Path;
        if (!string.IsNullOrEmpty(accessToken) &&
            (path.StartsWithSegments("/hubs")))
        {
            // Read the token out of the query string
            context.Token = accessToken;
            
            return Task.CompletedTask;
        }
        
        return base.MessageReceived(context);
    }
}