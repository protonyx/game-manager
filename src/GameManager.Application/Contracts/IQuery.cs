using GameManager.Application.Errors;

namespace GameManager.Application.Contracts;

public interface IQuery<TResponse> : IRequest<Result<TResponse, ApplicationError>>
{
    
}