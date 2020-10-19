using CloudObjects.Models;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CloudObjects.App
{
    public class DataController : ControllerBase
    {
        public DataController(DapperCX<long, SystemUser> data)
        {
            Data = data;
        }

        public DapperCX<long, SystemUser> Data { get; }

        protected async Task<IActionResult> TryOnVerified(string accountName, string accountKey, Func<long, Task<object>> action)
        {
            try
            {
                var acctId = await VerifyAccountId(accountName, accountKey);
                var result = await action.Invoke(acctId);
                return Ok(result);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        protected async Task<IActionResult> TryAny(Func<Task<object>> action)
        {
            try
            {
                var result = await action.Invoke();
                return Ok(result);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        protected async Task<long> VerifyAccountId(string accountName, string accountKey)
        {
            try
            {
                var acct = await Data.GetWhereAsync<Account>(new { name = accountName });
                if (acct == null) throw new Exception("Account name not found.");
                if (acct.Key.Equals(accountKey)) return acct.Id;
                throw new Exception("Missing or invalid account key.");
            }
            catch 
            {
                throw new Exception("Missing or invalid account key.");
            }
        }
    }
}
