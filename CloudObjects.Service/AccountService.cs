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
    public class AccountService
    {
        private readonly TokenGenerator _tokenGenerator;
        private readonly DapperCX<long> _data;        

        public AccountService(HttpContext httpContext, TokenGenerator tokenGenerator, DapperCX<long> data)
        {
            AccountId = httpContext?.GetAccountId() ?? 0;
            _tokenGenerator = tokenGenerator;
            _data = data;
        }

        public long AccountId { get; }

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
            await _data.InsertAsync(account);
            return new OkObjectResult(account);
        }

        public async Task<IActionResult> Put(string newName)
        {
            var acct = new Account()
            {
                Name = newName,
                Id = AccountId
            };
            await _data.UpdateAsync(acct);
            return new OkObjectResult(acct);
        }

        public async Task<IActionResult> Delete()
        {
            // note this works only if you don't have any objects in your account; delete does not cascade
            await new DeleteAccountActivity() { AccountId = AccountId }.ExecuteAsync(_data.GetConnection);            
            await _data.DeleteAsync<Account>(AccountId);
            return new OkResult();
        }

        private static string GetKey()
        {
            return StringId.New(50, StringIdRanges.Lower | StringIdRanges.Upper | StringIdRanges.Numeric | StringIdRanges.Special);
        }
    }
}
