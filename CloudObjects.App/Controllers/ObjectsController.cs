using CloudObjects.App.Extensions;
using CloudObjects.App.Queries;
using CloudObjects.App.Services;
using CloudObjects.Models;
using Dapper.CX.Classes;
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
    public class ObjectsController : CommonController
    {      
        public ObjectsController(   
            HttpContext httpContext,
            DapperCX<long> data) : base(httpContext, data)
        {
        }       

        [HttpPost]
        public async Task<IActionResult> Post(StoredObject @object)
        {
            @object.AccountId = AccountId;
            @object.Length = @object.Json.Length;
            await Data.InsertAsync(@object);
            return Ok(@object);
        }

        [HttpPut]
        public async Task<IActionResult> Put(StoredObject @object)
        {
            @object.AccountId = AccountId;
            @object.Length = @object.Json.Length;
            await Data.MergeAsync(@object);
            return Ok(@object);
        }

        [HttpPut]
        [Route("Rename")]
        public async Task<IActionResult> Put([FromQuery]string oldName, [FromQuery]string newName)
        {
            oldName = HttpUtility.UrlDecode(oldName);
            newName = HttpUtility.UrlDecode(newName);

            var obj = await Data.GetWhereAsync<StoredObject>(new { AccountId, Name = oldName });
            if (obj == null) return BadRequest($"Object named {oldName} in account Id {AccountId} not found.");

            var ct = new ChangeTracker<StoredObject>(obj);            
            obj.Name = newName;
            await Data.UpdateAsync(obj, ct);                            
            return Ok();
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> Get([FromRoute]string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = AccountId, name });
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpGet]
        [Route("Exists/{name}")]
        public async Task<IActionResult> Exists([FromRoute]string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.ExistsWhereAsync<StoredObject>(new { accountId = AccountId, name });
            return Ok(result);
        }

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = AccountId, name });
            if (result == null) return Ok();
            await Data.DeleteAsync<StoredObject>(result.Id);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            await Data.QueryAsync(new DeleteAllObjects() { AccountId = AccountId });
            return Ok();
        }

        [HttpPost]
        [Route("List")]
        public async Task<IActionResult> List(ListStoredObjects query)
        {
            query.AccountId = AccountId;
            var results = await Data.QueryAsync(query);
            return Ok(results);
        }
    }
}
