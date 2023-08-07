using GameManager.Application.Contracts.Queries;

namespace GameManager.Application.Queries;

public class NotFoundQueryResponse : IQueryResponse
{
    
}

public class NotFoundQueryResponse<TResult> : NotFoundQueryResponse, IQueryResponse<TResult>
{
    public TResult? Result => default;
}