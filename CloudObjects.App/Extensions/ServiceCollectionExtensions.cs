using CloudObjects.App.Services;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CloudObjects.App.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTokenGenerator(this IServiceCollection services, string jwtSecret)
        {
            services.AddScoped((sp) =>
            {
                var data = sp.GetRequiredService<DapperCX<long>>();
                return new TokenGenerator(jwtSecret, data);
            });
        }

        public static void AddHttpContext(this IServiceCollection services)
        {
            services.AddScoped((sp) =>
            {
                var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                return contextAccessor.HttpContext;
            });
        }
    }
}
