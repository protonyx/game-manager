using GameManager.Application.Errors;

namespace GameManager.Application.Contracts;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse, ApplicationError>>
    where TQuery : IQuery<TResponse>
{
    
}