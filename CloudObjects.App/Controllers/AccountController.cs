﻿using CloudObjects.App.Queries;
using CloudObjects.App.Services;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Dapper.CX.SqlServer.AspNetCore.Extensions;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StringIdLibrary;
using System;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : CommonController
    {
        private readonly TokenGenerator _tokenGenerator;

        public AccountController(
            HttpContext httpContext,
            TokenGenerator tokenGenerator,
            DapperCX<long> data) : base(httpContext, data)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [Route("Token")]
        [AllowAnonymous]
        public async Task<IActionResult> Token([FromBody] ApiCredentials login)
        {
            var result = await _tokenGenerator.GetTokenAsync(login.AccountName, login.AccountKey);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
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
        public async Task<IActionResult> Put([FromQuery] string newName)
        {
            var acct = new Account()
            {
                Name = newName,
                Id = AccountId
            };
            await Data.UpdateAsync(acct);
            return Ok(acct);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            // note this works only if you don't have any objects in your account; delete does not cascade
            await Data.QueryAsync(new DeleteAccountActivity() { AccountId = AccountId });
            await Data.DeleteAsync<Account>(AccountId);
            return Ok();
        }

        private static string GetKey()
        {
            return StringId.New(50, StringIdRanges.Lower | StringIdRanges.Upper | StringIdRanges.Numeric | StringIdRanges.Special);
        }       
    }
}
