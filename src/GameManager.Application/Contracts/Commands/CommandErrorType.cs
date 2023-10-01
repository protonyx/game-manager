namespace GameManager.Application.Contracts.Commands;

public enum CommandErrorType
{
    GeneralFailure,
    ValidationError,
    AuthorizationError,
    NotFoundError,
    Unknown
}