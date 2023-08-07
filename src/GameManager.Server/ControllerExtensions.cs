using GameManager.Application.Commands;
using GameManager.Application.Contracts.Commands;
using GameManager.Application.Contracts.Queries;
using GameManager.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace GameManager.Server;

public static class ControllerExtensions
{
    public static IActionResult GetActionResult(this ControllerBase controller, ICommandResponse commandResponse)
    {
        if (commandResponse is EntityCommandResponse data)
        {
            return controller.Ok(data.Value);
        }
        else if (commandResponse is SuccessfulCommandResponse success)
        {
            return controller.NoContent();
        }
        else if (commandResponse is FailureCommandResponse failure)
        {
            return controller.Problem(
                detail: failure.Reason,
                statusCode: StatusCodes.Status400BadRequest);
        }
        else if (commandResponse is AuthorizationErrorCommandResponse authorizationError)
        {
            return controller.Problem(
                detail: authorizationError.Reason,
                statusCode: StatusCodes.Status403Forbidden);
        }
        else if (commandResponse is ValidationErrorCommandResponse {Result.IsValid: false} validationError)
        {
            controller.ModelState.AddValidationResults(validationError.Result);

            return controller.ValidationProblem(controller.ModelState);
        }
        else if (commandResponse is NotFoundCommandResponse notFound)
        {
            return controller.NotFound();
        }

        return controller.Ok();
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