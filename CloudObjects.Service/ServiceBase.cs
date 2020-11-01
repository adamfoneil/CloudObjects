using CloudObjects.Service.Extensions;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace CloudObjects.Service
{
    public class ServiceBase
    {
        public ServiceBase(HttpContext httpContext, DapperCX<long> data)
        {
            if (httpContext?.User.Claims.Any() ?? false)
            {
                AccountId = httpContext.GetClaim(TokenGenerator.AccountIdClaim, (value) => Convert.ToInt64(value));
            }
            
            Data = data;                
        }

        protected long AccountId { get; }
        protected DapperCX<long> Data { get; }
    }
}
