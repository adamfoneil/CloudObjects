using CloudObjects.Client;
using CloudObjects.Client.Static;
using CloudObjects.Models;
using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Testing.Client.Static
{
    internal static class DbUtil
    {
        internal static void DeleteAccount(string testAccount, Func<IDbConnection> getConnection)
        {
            using (var cn = getConnection())
            {
                cn.Execute(
                    @"DELETE [act] 
                    FROM [dbo].[Activity] [act]
                    INNER JOIN [dbo].[Account] [a] ON [act].[AccountId]=[a].[Id]
                    WHERE [a].[Name]=@testAccount", new { testAccount });

                cn.Execute(
                    @"DELETE [so]
                    FROM [dbo].[StoredObject] [so]
                    INNER JOIN [dbo].[Account] [a] ON [so].[AccountId]=[a].[Id]
                    WHERE [a].[Name]=@testAccount", new { testAccount });

                cn.Execute("DELETE [a] FROM [dbo].[Account] [a] WHERE [Name]=@testAccount", new { testAccount });
            }
        }

        internal static async Task<Account> GetTestAccountAsync(string testAccount, Func<IDbConnection> getConnection)
        {
            using (var cn = getConnection.Invoke())
            {
                Account result = cn.QuerySingleOrDefault<Account>("SELECT * FROM [dbo].[Account] WHERE [Name]=@testAccount", new { testAccount });

                if (result == null)
                {
                    var client = new CloudObjectsAuthClient(HostLocations.Local);
                    result = await client.CreateAccountAsync(testAccount);
                }

                return result;
            }
        }
    }
}
