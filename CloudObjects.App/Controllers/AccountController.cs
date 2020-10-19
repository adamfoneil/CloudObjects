using CloudObjects.App.Queries;
using CloudObjects.Models;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.AspNetCore.Extensions;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Mvc;
using StringIdLibrary;
using System;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{    
    [ApiController]
    public class AccountController : DataController
    {
        public AccountController(DapperCX<long, SystemUser> data) : base(data)
        {
        }

        [HttpPost]
        [Route("api/[controller]")]
        public async Task<IActionResult> Post(string name)
        {
            var account = new Account()
            {
                Name = name,
                Key = GetKey(),
                InvoiceDate = DateTime.UtcNow.AddDays(30)
            };            
            await Data.InsertAsync(account);
            return Ok(account);
        }

        [HttpPut]
        [Route("api/[controller]/{accountName}")]
        public async Task<IActionResult> Put([FromRoute]string accountName, [FromQuery(Name = "key")]string accountKey, Account account) => 
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {            
                if (account.Id == 0) account.Id = acctId;
                await Data.UpdateAsync(account);
                return account;
            });

        [HttpDelete]
        [Route("api/[controller]/{accountName}")]
        public async Task<IActionResult> Delete([FromRoute] string accountName, [FromQuery(Name = "key")] string accountKey) =>
            await TryOnVerified(accountName, accountKey, async (acctId) =>
            {
                await Data.QueryAsync(new DeleteAccountActivity() { AccountId = acctId });
                await Data.DeleteAsync<Account>(acctId);
                return true;
            });

        private static string GetKey()
        {
            return StringId.New(50, StringIdRanges.Lower | StringIdRanges.Upper | StringIdRanges.Numeric | StringIdRanges.Special);
        }       
    }
}
