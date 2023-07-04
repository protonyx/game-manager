using GameManager.Application.Commands;
using GameManager.Application.Contracts.Commands;
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
}