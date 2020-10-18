using CloudObjects.Models;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoredObjectController : DataController
    {
        public StoredObjectController(DapperCX<long, SystemUser> data) : base(data)
        {
        }

        [HttpPut]
        [Route("api/[controller]/{accountName}")]
        public async Task<IActionResult> Post([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, StoredObject model) =>
            await DataActionAsync(model, async () =>
            {
                var acctId = await VerifyAccountId(accountName, accountKey);
                model.AccountId = acctId;
                model.Length = model.Json.Length;
                await Data.InsertAsync(model);
                return model;
            });        

        [HttpPut]
        [Route("api/[controller]/{accountName}")]
        public async Task<IActionResult> Put([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey, StoredObject model) =>
            await DataActionAsync(model, async () =>
            {
                var acctId = await VerifyAccountId(accountName, accountKey);
                model.AccountId = acctId;
                model.Length = model.Json.Length;
                await Data.MergeAsync(model);
                return model;
            });

    }
}
