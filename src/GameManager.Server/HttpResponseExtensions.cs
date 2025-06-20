using GameManager.Domain.ValueObjects;
using Microsoft.Net.Http.Headers;

namespace GameManager.Server;

public static class HttpResponseExtensions
{
    public static void SetETag(this HttpResponse response, ETag etag)
    {
        response.Headers[HeaderNames.ETag] = etag.ToString();
    }
}