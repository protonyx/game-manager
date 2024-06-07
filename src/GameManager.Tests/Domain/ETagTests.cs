using GameManager.Domain.ValueObjects;

namespace GameManager.Tests.Domain;

public class ETagTests
{
    [Fact]
    public void ETag_Tests()
    {
        Assert.Equal(ETag.From("\"123\""), ETag.From("123"));
        Assert.Equal(ETag.From("W/\"123\""), ETag.FromWeak("123"));
    }
}