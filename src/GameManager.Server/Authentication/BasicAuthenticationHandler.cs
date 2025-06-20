using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using GameManager.Application.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GameManager.Server.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationHandlerOptions>
{
    public const string BasicAuthenticationSchemeName = "Basic";

    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationHandlerOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder)
        : base(options, loggerFactory, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();
        if (authorizationHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorizationHeader.Substring("Basic ".Length).Trim();
            var credentialsAsEncodedString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var credentials = credentialsAsEncodedString.Split(':');

            if (credentials[0].Equals(Options.AdminUsername)
                && credentials[1].Equals(Options.AdminPassword))
            {
                var identityBuilder = new PlayerIdentityBuilder("Basic");
                identityBuilder.AddAdminRole();

                var identity = identityBuilder.Build();
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }
        }

        //Response.StatusCode = 401;
        //Response.Headers.Append("WWW-Authenticate", "Basic realm=\"game-manager\"");
        return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
    }
}