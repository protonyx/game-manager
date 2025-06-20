using Microsoft.AspNetCore.Authentication;

namespace GameManager.Server.Authentication;

public class BasicAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public string AdminUsername { get; set; } = "admin";

    public string AdminPassword { get; set; } = "password";
}