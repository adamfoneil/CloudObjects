using CloudObjects.Models;
using CloudObjects.Service.Extensions;
using CloudObjects.Service.Models;
using CloudObjects.Service.Queries;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StringIdLibrary;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudObjects.Service
{
    public class AccountService : ServiceBase
    {
        private readonly TokenGenerator _tokenGenerator;        

        public AccountService(HttpContext httpContext, TokenGenerator tokenGenerator, DapperCX<long> data) : base(httpContext, data)
        {        
            _tokenGenerator = tokenGenerator;

        }

        public async Task<IActionResult> Token(ApiCredentials login)
        {
            var result = await _tokenGenerator.GetTokenAsync(login.AccountName, login.AccountKey);
            return new OkObjectResult(result);
        }

        public async Task<IActionResult> Post(string name)
        {
            var account = new Account()
            {
                Name = name,
                Key = GetKey(),
                InvoiceDate = DateTime.UtcNow.AddDays(30)
            };
            await Data.InsertAsync(account);
            return new OkObjectResult(account);
        }

        public async Task<IActionResult> Put(string newName)
        {
            var acct = new Account()
            {
                Name = newName,
                Id = AccountId
            };
            await Data.UpdateAsync(acct);
            return new OkObjectResult(acct);
        }

        public async Task<IActionResult> Delete()
        {
            // note this works only if you don't have any objects in your account; delete does not cascade
            await new DeleteAccountActivity() { AccountId = AccountId }.ExecuteAsync(Data.GetConnection);            
            await Data.DeleteAsync<Account>(AccountId);
            return new OkResult();
        }

        private static string GetKey()
        {
            return StringId.New(50, StringIdRanges.Lower | StringIdRanges.Upper | StringIdRanges.Numeric | StringIdRanges.Special);
        }
    }
}
