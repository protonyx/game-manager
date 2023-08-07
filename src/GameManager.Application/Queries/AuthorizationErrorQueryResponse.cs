using GameManager.Application.Contracts.Queries;

namespace GameManager.Application.Queries;

public class AuthorizationErrorQueryResponse : IQueryResponse
{
    public string Reason { get; set; } = string.Empty;

    public AuthorizationErrorQueryResponse(string reason = "")
    {
        Reason = reason;
    }
}

public class AuthorizationErrorQueryResponse<TResult> : AuthorizationErrorQueryResponse, IQueryResponse<TResult>
{
    public TResult? Result => default;

    public AuthorizationErrorQueryResponse(string reason = "")
        : base(reason)
    {
    }
}