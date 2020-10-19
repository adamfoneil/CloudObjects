using CloudObjects.App.Queries;
using CloudObjects.Models;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.AspNetCore.Extensions;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{    
    [ApiController]
    public class StoredObjectController : DataController
    {
        public StoredObjectController(DapperCX<long, SystemUser> data) : base(data)
        {
        }

        [HttpPost]
        [Route("api/[controller]/{accountName}")]
        public async Task<IActionResult> Post([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, StoredObject model) =>
            await TryAny(async () =>
            {
                await PreSaveInner(accountName, accountKey, model);
                await Data.InsertAsync(model);
                return model;
            });

        [HttpGet]
        [Route("api/[controller]/{accountName}/name")]
        public async Task<IActionResult> GetByName([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, string name) =>
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {
                var result = await Data.GetWhereAsync<StoredObject>(new { accountId = acctId, name });
                if (result == null) return BadRequest();
                return result;
            });

        [HttpGet]
        [Route("api/[controller]/{accountName}/id")]
        public async Task<IActionResult> GetById([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, long id) =>
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {
                var result = await Data.GetAsync<StoredObject>(id);
                if (result == null) return BadRequest();
                if (result.AccountId != acctId) return BadRequest();
                return result;
            });

        [HttpGet]
        [Route("api/[controller]/{accountName}/list")]
        public async Task<IActionResult> List([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, ListStoredObjects query) =>
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {
                query.AccountId = acctId;                
                return await Data.QueryAsync(query);
            });

        [HttpPut]
        [Route("api/[controller]/{accountName}")]
        public async Task<IActionResult> Put([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, StoredObject model) =>
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {
                await PreSaveInner(accountName, accountKey, model);
                await Data.MergeAsync(model);
                return model;
            });

        [HttpDelete]
        [Route("api/[controller]/{accountName}/id")]
        public async Task<IActionResult> DeleteById([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, long id) =>
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {
                var result = await Data.GetAsync<StoredObject>(id);
                if (result == null) return BadRequest();
                if (result.AccountId != acctId) return BadRequest();
                await Data.DeleteAsync<StoredObject>(id);
                return true;
            });

        [HttpDelete]
        [Route("api/[controller]/{accountName}/name")]
        public async Task<IActionResult> DeleteByName([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, string name) =>
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {
                var result = await Data.GetWhereAsync<StoredObject>(new { accountId = acctId, name });
                if (result == null) return BadRequest();
                await Data.DeleteAsync<StoredObject>(result.Id);
                return true;
            });

        private async Task PreSaveInner(string accountName, string accountKey, StoredObject model)
        {
            var acctId = await VerifyAccountId(accountName, accountKey);
            model.AccountId = acctId;
            model.Length = model.Json.Length;
        }
    }
}
