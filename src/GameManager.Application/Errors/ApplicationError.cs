using FluentValidation.Results;

namespace GameManager.Application.Errors;

public class ApplicationError
{
    public ApplicationErrorType ErrorType { get; }

    public string? ErrorCode { get; }

    public string? Reason { get; private set; }

    public ValidationResult? ValidationResult { get; private set; }

    public ApplicationError(ApplicationErrorType errorType, string? errorCode = null)
    {
        ErrorType = errorType;
        ErrorCode = errorCode;
    }

    public static ApplicationError Failure(string reason, string? errorCode = null)
    {
        return new ApplicationError(ApplicationErrorType.GeneralFailure, errorCode)
        {
            Reason = reason
        };
    }

    public static ApplicationError Validation(string message, string? errorCode = null)
    {
        return new ApplicationError(ApplicationErrorType.ValidationError, errorCode)
        {
            Reason = message
        };
    }

    public static ApplicationError Validation<T>(ValidationResult validationResults, string? errorCode = null)
    {
        return new ApplicationError(ApplicationErrorType.ValidationError, errorCode)
        {
            Reason = $"{typeof(T).Name} has validation errors",
            ValidationResult = validationResults
        };
    }

    public static ApplicationError NotFound<T>(string? entityId = null, string? errorCode = null)
    {
        return NotFound(typeof(T).Name, entityId, errorCode);
    }

    public static ApplicationError NotFound(string entityType, string? entityId = null, string? errorCode = null)
    {
        return new ApplicationError(ApplicationErrorType.NotFoundError, errorCode)
        {
            Reason = string.IsNullOrWhiteSpace(entityId)
                ? $"{entityType} was not found"
                : $"{entityType} with ID {entityId} was not found"
        };
    }

    public static ApplicationError Authorization(string? reason = null, string? errorCode = null)
    {
        return new ApplicationError(ApplicationErrorType.AuthorizationError, errorCode)
        {
            Reason = reason
        };
    }
}