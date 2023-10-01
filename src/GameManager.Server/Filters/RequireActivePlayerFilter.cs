using GameManager.Application.Authorization;
using GameManager.Application.Contracts.Persistence;
using GameManager.Application.Features.Games;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GameManager.Server.Filters;

public class RequireActivePlayerFilter : IAsyncAuthorizationFilter
{
    private readonly IPlayerRepository _playerRepository;

    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public RequireActivePlayerFilter(
        IPlayerRepository playerRepository,
        ProblemDetailsFactory problemDetailsFactory)
    {
        _playerRepository = playerRepository;
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Check that the user represents an active player
        var playerId = context.HttpContext.User.GetPlayerId();

        if (!playerId.HasValue)
        {
            return;
        }

        var player = await _playerRepository.GetByIdAsync(playerId.Value);

        if (player is not {Active: true})
        {
            var pd = _problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                statusCode: StatusCodes.Status403Forbidden,
                type: GameErrors.ErrorCodes.PlayerInvalidState,
                detail: "Player is not active");
                
            context.Result = new ObjectResult(pd)
            {
                StatusCode = pd.Status
            };
        }
    }
}