using CloudObjects.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CloudObjects.App.Data;
using Microsoft.EntityFrameworkCore;

namespace CloudObjects.App.Services
{
    public class TokenGenerator
    {
        private readonly string _jwtSecret;
        private readonly CloudObjectsDbContext _dbContext;

        public const string AccountIdClaim = "AccountId";

        public TokenGenerator(
            string jwtSecret,
            CloudObjectsDbContext dbContext)
        {
            _jwtSecret = jwtSecret;
            _dbContext = dbContext;
        }

        public async Task<Account> ValidateAccountAsync(string accountName, string accountKey)
        {
            var result = await _dbContext.Accounts.FirstOrDefaultAsync(e => e.Name == accountName);
            return (result?.Key.Equals(accountKey) ?? false) ? result : default;
        }

        public async Task<string> GetTokenAsync(string accountName, string accountKey)
        {
            var account = await ValidateAccountAsync(accountName, accountKey);
            if (account != null) return GenerateToken(account);
            throw new System.Exception("Account name or key was not valid.");
        }

        private string GenerateToken(Account account)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(AccountIdClaim, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(365)).ToUnixTimeSeconds().ToString()),
            };

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)), SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
