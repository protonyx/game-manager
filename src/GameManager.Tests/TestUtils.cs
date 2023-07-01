using System.Security.Claims;
using AutoFixture.AutoMoq;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts;

namespace GameManager.Tests;

public static class TestUtils
{
    public static IFixture GetTestFixture()
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());

        return fixture;
    }
}