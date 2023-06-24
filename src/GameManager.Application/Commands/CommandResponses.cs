using FluentValidation.Results;

namespace GameManager.Application.Commands;

public static class CommandResponses
{
    public static EntityCommandResponse Data(Guid id, object data)
    {
        return new EntityCommandResponse(id, data);
    }

    public static SuccessfulCommandResponse Success()
    {
        return new SuccessfulCommandResponse();
    }

    public static FailureCommandResponse Failure(string reason)
    {
        return new FailureCommandResponse(reason);
    }

    public static ValidationErrorCommandResponse ValidationError(ValidationResult result)
    {
        return new ValidationErrorCommandResponse(result);
    }

    public static AuthorizationErrorCommandResponse AuthorizationError()
    {
        return new AuthorizationErrorCommandResponse();
    }

    public static NotFoundCommandResponse NotFound()
    {
        return new NotFoundCommandResponse();
    }
}