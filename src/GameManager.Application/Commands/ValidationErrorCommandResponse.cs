using FluentValidation.Results;

namespace GameManager.Application.Commands;

public class ValidationErrorCommandResponse : ICommandResponse
{
    public ValidationResult Result { get; }

    public ValidationErrorCommandResponse(ValidationResult result)
    {
        Result = result;
    }
}