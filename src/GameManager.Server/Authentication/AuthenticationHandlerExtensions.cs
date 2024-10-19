using Microsoft.AspNetCore.Authentication;

namespace GameManager.Server.Authentication;

public static class AuthenticationHandlerExtensions
{
    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder,
        Action<BasicAuthenticationHandlerOptions> configureOptions) =>
        builder.AddBasic("Basic", "Basic", configureOptions);
    
    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder,
        string authenticationScheme,
        string? displayName,
        Action<BasicAuthenticationHandlerOptions> configureOptions) =>
        builder.AddScheme<BasicAuthenticationHandlerOptions, BasicAuthenticationHandler>(authenticationScheme,
            displayName, configureOptions);
}