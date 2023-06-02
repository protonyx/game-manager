using FluentValidation.Results;

namespace GameManager.Application.Features.Games.Commands;

public class ValidateCommandResponseBase
{
    public ValidationResult? ValidationResult { get; set; }

}