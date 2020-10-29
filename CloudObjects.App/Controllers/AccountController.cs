﻿using CloudObjects.App.Queries;
using CloudObjects.App.Services;
using CloudObjects.Client.Models;
using CloudObjects.Models;
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
        private readonly TokenGenerator _tokenGenerator;

        public AccountController(
            TokenGenerator tokenGenerator,
            DapperCX<long> data) : base(data)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [Route("api/[controller]/Token")]        
        public async Task<IActionResult> Token([FromBody] ApiCredentials login)
        {
            string token = await _tokenGenerator.GetTokenAsync(login.AccountName, login.AccountKey);
            return Ok(token);
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
