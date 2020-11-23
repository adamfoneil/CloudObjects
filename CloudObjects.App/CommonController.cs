using CloudObjects.App.Extensions;
using CloudObjects.App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CloudObjects.App
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommonController : ControllerBase
    {
        public CommonController(
            HttpContext httpContext)
        {            
            if (httpContext?.User.Claims.Any() == true)
            {
                AccountId = httpContext.GetClaim(TokenGenerator.AccountIdClaim, Convert.ToInt64);
            }
        }

        protected long AccountId { get; }
    }
}
