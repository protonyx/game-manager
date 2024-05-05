using System.Diagnostics;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GameManager.Server.Authorization;

public class ProblemAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
    
    public Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded)
        {
            return next(context);
        }
        
        return Handle();

        async Task Handle()
        {
            if (authorizeResult.Challenged)
            {
                if (policy.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ChallengeAsync(scheme);
                    }
                }
                else
                {
                    await context.ChallengeAsync();
                }
            }
            else if (authorizeResult.Forbidden)
            {
                if (policy.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ForbidAsync(scheme);
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    var problem = new ProblemDetails()
                    {
                        TraceId = Activity.Current.Id,
                        Status = StatusCodes.Status403Forbidden,
                        Detail = authorizeResult.AuthorizationFailure?.FailureReasons.FirstOrDefault()?.Message
                    };
                    await problem.ExecuteAsync(context);
                }
            }
        }
    }
}