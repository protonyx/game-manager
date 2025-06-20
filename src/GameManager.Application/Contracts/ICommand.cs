using GameManager.Application.Errors;

namespace GameManager.Application.Contracts;

public interface ICommand : IRequest<UnitResult<ApplicationError>>
{

}

public interface ICommand<TResponse> : IRequest<Result<TResponse, ApplicationError>>
{

}