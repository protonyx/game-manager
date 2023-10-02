using FluentValidation.Results;

namespace GameManager.Application.Contracts.Commands;

public class CommandError
{
    public CommandErrorType ErrorType { get; }

    public string? ErrorCode { get; }
    
    public string? Reason { get; private set; }

    public ValidationResult? ValidationResult { get; private set; }

    public CommandError(CommandErrorType errorType, string? errorCode = null)
    {
        ErrorType = errorType;
        ErrorCode = errorCode;
    }

    public static CommandError Failure(string reason, string? errorCode = null)
    {
        return new CommandError(CommandErrorType.GeneralFailure, errorCode)
        {
            Reason = reason
        };
    }

    public static CommandError Validation(string message, string? errorCode = null)
    {
        return new CommandError(CommandErrorType.ValidationError, errorCode)
        {
            Reason = message
        };
    }

    public static CommandError Validation<T>(ValidationResult validationResults, string? errorCode = null)
    {
        return new CommandError(CommandErrorType.ValidationError, errorCode)
        {
            Reason = $"{typeof(T).Name} has validation errors",
            ValidationResult = validationResults
        };
    }

    public static CommandError NotFound<T>(string? entityId = null, string? errorCode = null)
    {
        return NotFound(typeof(T).Name, entityId, errorCode);
    }

    public static CommandError NotFound(string entityType, string? entityId = null, string? errorCode = null)
    {
        return new CommandError(CommandErrorType.NotFoundError, errorCode)
        {
            Reason = string.IsNullOrWhiteSpace(entityId)
                ? $"{entityType} was not found"
                : $"{entityType} with ID {entityId} was not found"
        };
    }

    public static CommandError Authorization(string? reason = null, string? errorCode = null)
    {
        return new CommandError(CommandErrorType.AuthorizationError, errorCode)
        {
            Reason = reason
        };
    }
}