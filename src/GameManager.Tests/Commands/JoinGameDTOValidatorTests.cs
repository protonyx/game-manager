using GameManager.Domain.Constants;
using GameManager.Server.Endpoints.Games;

namespace GameManager.Tests.Commands;

public class JoinGameDTOValidatorTests
{
    private readonly JoinGameDTOValidator _validator = new();

    [Fact]
    public async Task Validator_WithValidNameAndEntryCode_IsValid()
    {
        // Arrange
        var dto = new JoinGameDTO
        {
            EntryCode = "ABCD",
            Name = "Alice",
            Color = PlayerColors.All[0]
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_WithNullColor_IsValid()
    {
        // Arrange
        var dto = new JoinGameDTO
        {
            EntryCode = "ABCD",
            Name = "Alice",
            Color = null
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validator_WithValidPaletteColor_IsValid()
    {
        // Arrange
        foreach (var color in PlayerColors.All)
        {
            var dto = new JoinGameDTO
            {
                EntryCode = "ABCD",
                Name = "Alice",
                Color = color
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeTrue($"color {color} should be valid");
        }
    }

    [Fact]
    public async Task Validator_WithColorNotInPalette_IsInvalid()
    {
        // Arrange
        var dto = new JoinGameDTO
        {
            EntryCode = "ABCD",
            Name = "Alice",
            Color = "#FFFFFF"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Color");
    }

    [Fact]
    public async Task Validator_WithEmptyEntryCode_IsInvalid()
    {
        // Arrange
        var dto = new JoinGameDTO
        {
            EntryCode = "",
            Name = "Alice",
            Color = PlayerColors.All[0]
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EntryCode");
    }

    [Fact]
    public async Task Validator_WhenObserver_ColorNotRequired()
    {
        // Arrange
        var dto = new JoinGameDTO
        {
            EntryCode = "ABCD",
            Name = "",
            Observer = true,
            Color = "#FFFFFF" // invalid color but observer so it's skipped
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
