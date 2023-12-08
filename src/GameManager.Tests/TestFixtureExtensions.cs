using System.Security.Claims;
using AutoFixture.Dsl;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests;

public static class TestFixtureExtensions
{
    public static void SetUser(this IFixture fixture, Action<IPlayerIdentityBuilder> builderAction)
    {
        var principal = PlayerIdentityBuilder.CreatePrincipal(builderAction);

        var userContext = fixture.Freeze<Mock<IUserContext>>();
        userContext.Setup(x => x.User).Returns(principal);
    }

    public static IPostprocessComposer<Player> BuildPlayer(this IFixture fixture, Game game)
    {
        return fixture.Build<Player>()
            .FromFactory(() =>
            {
                var playerName = PlayerName.From(fixture.Create<string>().Substring(0, 10));
                return new Player(playerName.Value, game);
            });
    }
}