---
name: backend-testing
description: Guidelines and instructions for testing the backend of the Game Manager application. Use this when writing tests for the backend components, including unit tests, integration tests, and API endpoint tests.
---

## Libraries

The backend uses xUnit v3 as the testing framework with additional libraries:
- AutoFixture and AutoMoq for test data generation and mocking
- Use Moq for mocking dependencies
- FluentAssertions for more readable assertions
- FastEndpoints.Testing for API endpoint testing
- Microsoft.AspNetCore.Mvc.Testing for integration testing

## Testing Standards

- Follow AAA pattern (Arrange, Act, Assert)
- Test both success and failure scenarios
- Include null parameter validation tests

## Guidelines

Use these guidelines when writing tests:
- All test dependencies must be registered with the test fixture by calling `fixture.Freeze` with the `Mock` type as the type argument
- Never mock `IUserContext`, instead use the `SetUser` extension for AutoFixture to configure the User Principal:
  ```
  fixture.SetUser(user =>
  {
    user.AddGameId(game.Id)
        .AddPlayerId(player.Id);
  })
  ```

#### Adding New Tests

1. Create a new test class in the appropriate directory under `src/GameManager.Tests/`
2. Use the `[Fact]` attribute for test methods
3. Follow the Arrange-Act-Assert pattern
4. Use FluentAssertions for assertions

Example:
```csharp
using FluentAssertions;

namespace GameManager.Tests;

public class SimpleTest
{
    [Fact]
    public void SimpleTestExample_ShouldPass()
    {
        // Arrange
        var testString = "Hello, World!";
        
        // Act
        var result = testString.ToUpper();
        
        // Assert
        result.Should().Be("HELLO, WORLD!");
    }
}
```