using Microsoft.AspNetCore.Http;

namespace CloudObjects.App.Extensions
{
    public static class RequestExtensions
    {
        public static bool IsLocal(this HttpRequest request) => request.Host.Value.StartsWith("localhost:");
    }
}
