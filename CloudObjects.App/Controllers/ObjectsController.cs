using CloudObjects.App.Extensions;
using CloudObjects.App.Queries;
using CloudObjects.App.Services;
using CloudObjects.Models;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.AspNetCore.Extensions;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ObjectsController : DataController
    {      
        private readonly long _accountId;

        public ObjectsController(DapperCX<long, SystemUser> data) : base(data)
        {            
            _accountId = this.GetClaim(TokenGenerator.AccountIdClaim, (value) => Convert.ToInt64(value));
        }

        [HttpPost]
        public async Task<IActionResult> Post(StoredObject @object)
        {
            @object.AccountId = _accountId;
            await Data.InsertAsync(@object);
            return Ok(@object);
        }

        [HttpPut]
        public async Task<IActionResult> Put(StoredObject @object)
        {
            @object.AccountId = _accountId;
            await Data.MergeAsync(@object);
            return Ok(@object);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = _accountId, name });
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string name)
        {
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = _accountId, name });
            if (result == null) return BadRequest();
            await Data.DeleteAsync(result.Id);
            return Ok();
        }

        [HttpGet]
        [Route("/list")]
        public async Task<IActionResult> List(ListStoredObjects query)
        {
            query.AccountId = _accountId;
            var results = await Data.QueryAsync(query);
            return Ok(results);
        }
    }
}
