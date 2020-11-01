using CloudObjects.Models;
using CloudObjects.Service.Models;
using CloudObjects.Service.Queries;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Mvc;
using StringIdLibrary;
using System;
using System.Threading.Tasks;

namespace CloudObjects.Service
{
    public class AccountService
    {
        private readonly TokenGenerator _tokenGenerator;
        private readonly DapperCX<long> _data;
        private readonly long _accountId;

        public AccountService(long accountId, TokenGenerator tokenGenerator, DapperCX<long> data)
        {
            _accountId = accountId;
            _tokenGenerator = tokenGenerator;
            _data = data;
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
            await _data.InsertAsync(account);
            return new OkObjectResult(account);
        }

        public async Task<IActionResult> Put(string newName)
        {
            var acct = new Account()
            {
                Name = newName,
                Id = _accountId
            };
            await _data.UpdateAsync(acct);
            return new OkObjectResult(acct);
        }

        public async Task<IActionResult> Delete()
        {
            // note this works only if you don't have any objects in your account; delete does not cascade
            await new DeleteAccountActivity() { AccountId = _accountId }.ExecuteAsync(_data.GetConnection);            
            await _data.DeleteAsync<Account>(_accountId);
            return new OkResult();
        }

        private static string GetKey()
        {
            return StringId.New(50, StringIdRanges.Lower | StringIdRanges.Upper | StringIdRanges.Numeric | StringIdRanges.Special);
        }
    }
}
