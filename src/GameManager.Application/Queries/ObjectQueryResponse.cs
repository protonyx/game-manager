using GameManager.Application.Contracts.Queries;

namespace GameManager.Application.Queries;

public class ObjectQueryResponse : IQueryResponse
{
    public object Result { get; set; }

    public ObjectQueryResponse(object result)
    {
        Result = result;
    }
}

public class ObjectQueryResponse<TResult> : ObjectQueryResponse, IQueryResponse<TResult>
{
    public TResult? Result { get; }

    public ObjectQueryResponse(TResult? result)
        : base(result)
    {
        Result = result;
    }
}