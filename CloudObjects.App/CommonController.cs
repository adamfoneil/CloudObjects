using CloudObjects.App.Extensions;
using CloudObjects.App.Services;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudObjects.App
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommonController : ControllerBase
    {
        public CommonController(
            HttpContext httpContext,
            DapperCX<long> data)
        {            
            if (httpContext?.User.Claims.Any() ?? false)
            {
                AccountId = httpContext.GetClaim(TokenGenerator.AccountIdClaim, (value) => Convert.ToInt64(value));
            }
            
            Data = data;
        }

        protected long AccountId { get; }
        protected DapperCX<long> Data { get; }

        protected async Task<IActionResult> TryAsync<T>(Func<Task<T>> func)
        {
            try
            {
                var result = await func.Invoke();
                return Ok(result);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}
