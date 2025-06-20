using GameManager.Application.Errors;

namespace GameManager.Application.Contracts;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, UnitResult<ApplicationError>>
    where TCommand : ICommand
{

}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse, ApplicationError>>
    where TCommand : ICommand<TResponse>
{

}