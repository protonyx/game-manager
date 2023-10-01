using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Queries;
using GameManager.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server;

public static class ControllerExtensions
{
    public static IActionResult GetErrorActionResult(this ControllerBase controller, CommandError error)
    {
        switch (error.ErrorType)
        {
            case CommandErrorType.GeneralFailure:
                return controller.Problem(
                    type: error.ErrorCode,
                    title: error.ErrorType.ToString(),
                    detail: error.Reason,
                    statusCode: StatusCodes.Status400BadRequest);
            case CommandErrorType.ValidationError:
                controller.ModelState.AddValidationResults(error.ValidationResult!);
                
                return controller.ValidationProblem(
                    type: error.ErrorCode,
                    title: error.ErrorType.ToString(),
                    detail: error.Reason,
                    statusCode: StatusCodes.Status400BadRequest,
                    modelStateDictionary: controller.ModelState);
            case CommandErrorType.AuthorizationError:
                return controller.Problem(
                    type: error.ErrorCode,
                    title: error.ErrorType.ToString(),
                    detail: error.Reason,
                    statusCode: StatusCodes.Status403Forbidden);
            case CommandErrorType.NotFoundError:
                return controller.Problem(
                    type: error.ErrorType.ToString(),
                    statusCode: StatusCodes.Status404NotFound);
            case CommandErrorType.Unknown:
                return controller.Problem(
                    type: error.ErrorCode,
                    title: error.ErrorType.ToString(),
                    detail: error.Reason,
                    statusCode: StatusCodes.Status400BadRequest);
            default:
                throw new ArgumentOutOfRangeException();
        }

        return controller.NoContent();
    }
    
    public static IActionResult GetActionResult(this ControllerBase controller, IQueryResponse queryResponse)
    {
        if (queryResponse is ObjectQueryResponse obj)
        {
            return controller.Ok(obj.Result);
        }
        else if (queryResponse is AuthorizationErrorQueryResponse authorizationError)
        {
            return controller.Problem(
                detail: authorizationError.Reason,
                statusCode: StatusCodes.Status403Forbidden);
        }
        else if (queryResponse is NotFoundQueryResponse notFound)
        {
            return controller.NotFound();
        }

        return controller.Ok();
    }
}