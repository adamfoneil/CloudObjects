using CloudObjects.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CloudObjects.Service
{
    public class AccountService
    {
        public AccountService(TokenGenerator)
        {

        }
        public async Task<IActionResult> Token(ApiCredentials credentials)
        {
            var result = await _tokenGenerator.GetTokenAsync(login.AccountName, login.AccountKey);
            return Ok(result);
        }
    }
}
