using GameManager.Application.Contracts.Queries;

namespace GameManager.Application.Queries;

public static class QueryResponses
{
    public static IQueryResponse Object(object value) => new ObjectQueryResponse(value);
    
    public static IQueryResponse<T> Object<T>(T value) => new ObjectQueryResponse<T>(value);
    
    public static IQueryResponse NotFound() => new NotFoundQueryResponse();
    
    public static IQueryResponse<T> NotFound<T>() => new NotFoundQueryResponse<T>();

    public static IQueryResponse AuthorizationError(string reason = "") => new AuthorizationErrorQueryResponse(reason);
    
    public static IQueryResponse<T> AuthorizationError<T>(string reason = "") => new AuthorizationErrorQueryResponse<T>(reason);
}