using CloudObjects.App.Services;
using CloudObjects.Client.Models;
using CloudObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CloudObjects.App.Interfaces;
using Microsoft.AspNetCore.JsonPatch;

namespace CloudObjects.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : CommonController
    {
        private readonly TokenGenerator _tokenGenerator;
        private readonly IAccountService _accountService;

        public AccountController(
            HttpContext httpContext,
            TokenGenerator tokenGenerator,
            IAccountService accountService) : base(httpContext)
        {
            _tokenGenerator = tokenGenerator;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("Token")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Token([FromBody] ApiCredentials login)
            => await _tokenGenerator.GetTokenAsync(login.AccountName, login.AccountKey);

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Account>> Post(string name)
        {
            var account = await _accountService.CreateAsync(name);
            return Created(string.Empty, account);
        }

        [HttpPut, Obsolete("Please use PATCH method")]
        public async Task<ActionResult<Account>> Put([FromQuery] string newName)
            => await _accountService.RenameAsync(AccountId, newName);

        [HttpPatch]
        public async Task<ActionResult<Account>> Patch([FromBody] JsonPatchDocument<Account> patchDocument)
            => await _accountService.PatchAsync(AccountId, patchDocument);

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await _accountService.DeleteAsync(AccountId);
            return NoContent();
        }
    }
}
