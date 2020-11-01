using CloudObjects.Models;
using CloudObjects.Service;
using CloudObjects.Service.Models;
using CloudObjects.Service.Queries;
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
        private readonly AccountService _accountService;

        public AccountController(
            HttpContext httpContext,
            AccountService accountService,
            DapperCX<long> data) : base(httpContext, data)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [Route("Token")]
        [AllowAnonymous]
        public async Task<IActionResult> Token([FromBody] ApiCredentials login) => await _accountService.Token(login);

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(string name) => await _accountService.Post(name);

        [HttpPut]
        public async Task<IActionResult> Put([FromQuery] string newName) => await _accountService.Put(newName);

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            // note this works only if you don't have any objects in your account; delete does not cascade
            await Data.QueryAsync(new DeleteAccountActivity() { AccountId = AccountId });
            await Data.DeleteAsync<Account>(AccountId);
            return Ok();
        }
    }
}
