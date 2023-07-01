using GameManager.Application;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GameManager.Server.Filters;

public class RequireActivePlayerFilter : IAsyncAuthorizationFilter
{
    private readonly IPlayerRepository _playerRepository;

    public RequireActivePlayerFilter(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
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
            context.Result = new ForbidResult();
        }
    }
}