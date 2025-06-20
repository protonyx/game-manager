using GameManager.Application.Errors;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server;

public static class ControllerExtensions
{
    public static IActionResult GetErrorActionResult(this ControllerBase controller, ApplicationError error)
    {
        switch (error.ErrorType)
        {
            case ApplicationErrorType.GeneralFailure:
                return controller.Problem(
                    type: error.ErrorCode,
                    title: error.ErrorType.ToString(),
                    detail: error.Reason,
                    statusCode: StatusCodes.Status400BadRequest);
            case ApplicationErrorType.ValidationError:
                controller.ModelState.AddValidationResults(error.ValidationResult!);

                return controller.ValidationProblem(
                    type: error.ErrorCode,
                    title: error.ErrorType.ToString(),
                    detail: error.Reason,
                    statusCode: StatusCodes.Status400BadRequest,
                    modelStateDictionary: controller.ModelState);
            case ApplicationErrorType.AuthorizationError:
                return controller.Problem(
                    type: error.ErrorCode,
                    title: error.ErrorType.ToString(),
                    detail: error.Reason,
                    statusCode: StatusCodes.Status403Forbidden);
            case ApplicationErrorType.NotFoundError:
                return controller.Problem(
                    type: error.ErrorType.ToString(),
                    statusCode: StatusCodes.Status404NotFound);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}