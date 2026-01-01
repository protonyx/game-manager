using System.Security.Claims;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Application.Features.Games.Commands.CreateGame;
using GameManager.Domain.Entities;
using GameManager.Domain.ValueObjects;

namespace GameManager.Tests;

public static class TestUtils
{
    public static IFixture GetTestFixture()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        fixture.Register(() => new CreateTrackerDTO
        {
            Name = $"Tracker{Random.Shared.Next(1, 1000)}",
            StartingValue = 0
        });

        return fixture;
    }
}