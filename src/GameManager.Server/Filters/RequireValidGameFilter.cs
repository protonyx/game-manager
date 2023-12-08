using GameManager.Application.Authorization;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GameManager.Server.Filters;

public class RequireValidGameFilter : IAsyncAuthorizationFilter
{
    private readonly IGameRepository _gameRepository;

    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public RequireValidGameFilter(
        IGameRepository gameRepository,
        ProblemDetailsFactory problemDetailsFactory)
    {
        _gameRepository = gameRepository;
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var gameId = context.HttpContext.User.GetGameId();

        if (!gameId.HasValue)
        {
            return;
        }
        
        // Game must still be valid
        var gameExists = await _gameRepository.ExistsAsync(gameId.Value);

        if (!gameExists)
        {
            var pd = _problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                statusCode: StatusCodes.Status403Forbidden,
                type: GameErrors.ErrorCodes.GameInvalidState,
                detail: "Game is no longer valid");
                
            context.Result = new ObjectResult(pd)
            {
                StatusCode = pd.Status
            };
        }
    }
}