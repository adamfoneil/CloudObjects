﻿using CloudObjects.Models;
using Dapper.CX.Classes;
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
        public async Task<IActionResult> Post(Account account) => await DataActionAsync(account, async () =>
        {
            account.Key = GetKey();
            account.InvoiceDate = DateTime.UtcNow.AddDays(30);
            await Data.InsertAsync(account);
            return account;
        });

        [HttpPut]
        [Route("api/[controller]/{accountName}")]
        public async Task<IActionResult> Put([FromRoute]string accountName, [FromQuery(Name = "key")]string accountKey, Account account) => await DataActionAsync(account, async () =>
        {
            var acctId = await VerifyAccountId(accountName, accountKey);
            if (account.Id == 0) account.Id = acctId;            
            await Data.UpdateAsync(account);
            return account;                                    
        });

        private static string GetKey()
        {
            return StringId.New(50, StringIdRanges.Lower | StringIdRanges.Upper | StringIdRanges.Numeric | StringIdRanges.Special);
        }       
    }
}
