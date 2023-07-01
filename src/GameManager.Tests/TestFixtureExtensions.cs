using System.Security.Claims;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts;

namespace GameManager.Tests;

public static class TestFixtureExtensions
{
    public static void SetUser(this IFixture fixture, Action<IPlayerIdentityBuilder> builderAction)
    {
        var principal = PlayerIdentityBuilder.CreatePrincipal(builderAction);

        var userContext = fixture.Freeze<Mock<IUserContext>>();
        userContext.Setup(x => x.User).Returns(principal);
    }
}