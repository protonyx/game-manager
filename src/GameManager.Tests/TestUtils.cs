using AutoFixture.AutoMoq;

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