using System.Security.Claims;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using GameManager.Application.Authorization;
using GameManager.Application.Contracts;
using GameManager.Domain.Entities;

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