using CloudObjects.App.Extensions;
using CloudObjects.App.Queries;
using CloudObjects.App.Services;
using CloudObjects.Models;
using Dapper.CX.SqlServer.AspNetCore.Extensions;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Web;

namespace CloudObjects.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ObjectsController : ControllerBase
    {      
        private readonly long _accountId;

        public ObjectsController(   
            HttpContext httpContext,
            DapperCX<long> data) 
        {
            Data = data;
            _accountId = httpContext.GetClaim(TokenGenerator.AccountIdClaim, (value) => Convert.ToInt64(value));
        }

        public DapperCX<long> Data { get; }

        [HttpPost]
        public async Task<IActionResult> Post(StoredObject @object)
        {
            @object.AccountId = _accountId;
            @object.Length = @object.Json.Length;
            await Data.InsertAsync(@object);
            return Ok(@object);
        }

        [HttpPut]
        public async Task<IActionResult> Put(StoredObject @object)
        {
            @object.AccountId = _accountId;
            @object.Length = @object.Json.Length;
            await Data.MergeAsync(@object);
            return Ok(@object);
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> Get([FromRoute]string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = _accountId, name });
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpGet]
        [Route("exists/{name}")]
        public async Task<IActionResult> Exists([FromRoute]string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.ExistsWhereAsync<StoredObject>(new { accountId = _accountId, name });
            return Ok(result);
        }

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = _accountId, name });
            if (result == null) return BadRequest();
            await Data.DeleteAsync<StoredObject>(result.Id);
            return Ok();
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> List(ListStoredObjects query)
        {
            query.AccountId = _accountId;
            var results = await Data.QueryAsync(query);
            return Ok(results);
        }
    }
}
