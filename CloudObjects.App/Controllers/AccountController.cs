using CloudObjects.Models;
using CloudObjects.Service;
using CloudObjects.Service.Models;
using CloudObjects.Service.Queries;
using Dapper.CX.SqlServer.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {        
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
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
        public async Task<IActionResult> Delete() => await _accountService.Delete();
    }
}
