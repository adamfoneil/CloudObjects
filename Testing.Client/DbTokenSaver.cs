using CloudObjects.Service.Interfaces;
using Dapper.CX.Abstract;
using System;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace Testing.Client
{
    public class DbTokenSaver : DbDictionary<string>, ITokenSaver
    {
        public DbTokenSaver(Func<IDbConnection> getConnection) : base(getConnection, "dbo.OAuthToken")
        {
        }

        public async Task<string> GetAsync(string accountName) => await GetAsync<string>(accountName);

        public async Task SaveAsync(string accountName, string token) => await SetAsync(accountName, token);        

        protected override TValue Deserialize<TValue>(string value) => JsonSerializer.Deserialize<TValue>(value);

        protected override string Serialize<TValue>(TValue value) => JsonSerializer.Serialize(value);        
    }
}
