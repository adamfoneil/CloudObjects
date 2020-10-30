using CloudObjects.App.Extensions;
using CloudObjects.App.Services;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CloudObjects.App
{
    public class CommonController : ControllerBase
    {
        public CommonController(
            HttpContext httpContext,
            DapperCX<long> data)
        {
            if (User?.Claims.Any() ?? false)
            {
                AccountId = httpContext.GetClaim(TokenGenerator.AccountIdClaim, (value) => Convert.ToInt64(value));
            }
            
            Data = data;
        }

        protected long AccountId { get; }
        protected DapperCX<long> Data { get; }
    }
}
