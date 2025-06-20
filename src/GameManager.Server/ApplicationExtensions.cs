using FastEndpoints;
using GameManager.Application.Errors;

namespace GameManager.Server;

public static class ApplicationExtensions
{
    public static ProblemDetails ToProblemDetails(this ApplicationError error)
    {
        switch (error.ErrorType)
        {
            case ApplicationErrorType.GeneralFailure:
                return new FastEndpoints.ProblemDetails()
                {
                    Detail = error.Reason,
                    Instance = error.ErrorCode,
                    Status = StatusCodes.Status400BadRequest
                };
            case ApplicationErrorType.ValidationError:
                return new FastEndpoints.ProblemDetails(error.ValidationResult!.Errors,
                    StatusCodes.Status400BadRequest);
            case ApplicationErrorType.AuthorizationError:
                return new FastEndpoints.ProblemDetails()
                {
                    Detail = error.Reason,
                    Instance = error.ErrorCode,
                    Status = StatusCodes.Status403Forbidden
                };
            case ApplicationErrorType.NotFoundError:
                return new FastEndpoints.ProblemDetails()
                {
                    Detail = error.Reason,
                    Instance = error.ErrorCode,
                    Status = StatusCodes.Status404NotFound
                };
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}