namespace GameManager.Application.Contracts.Queries;

public interface IQueryResponse
{
    
}

public interface IQueryResponse<out TResult> : IQueryResponse
{
    TResult? Result { get; }
}