using GameManager.Application.Data;
using GameManager.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GameManager.Server.Authentication;

public class CustomJwtBearerEvents : JwtBearerEvents
{

    private readonly IGameRepository _gameRepository;

    public CustomJwtBearerEvents(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public override Task MessageReceived(MessageReceivedContext context)
    {
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

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var gameId = context.Principal?.GetGameId();

        if (gameId.HasValue)
        {
            // Game must still be valid
            var game = await _gameRepository.GetByIdAsync(gameId.Value);

            if (game == null)
            {
                context.Fail("Game is no longer valid");

                return;
            }
        }
        
        await base.TokenValidated(context);
    }
}