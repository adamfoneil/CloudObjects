using CloudObjects.App.Queries;
using CloudObjects.Models;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;

namespace CloudObjects.Service
{
    public class ObjectsService : ServiceBase
    {
        public ObjectsService(HttpContext httpContext, DapperCX<long> data) : base(httpContext, data)
        {
        }

        public async Task<IActionResult> Post(StoredObject @object)
        {
            @object.AccountId = AccountId;
            @object.Length = @object.Json.Length;
            await Data.InsertAsync(@object);
            return new OkObjectResult(@object);
        }

        public async Task<IActionResult> Put(StoredObject @object)
        {
            @object.AccountId = AccountId;
            @object.Length = @object.Json.Length;
            await Data.MergeAsync(@object);
            return new OkObjectResult(@object);
        }

        public async Task<IActionResult> Rename(string oldName, string newName)
        {
            oldName = HttpUtility.UrlDecode(oldName);
            newName = HttpUtility.UrlDecode(newName);

            var obj = await Data.GetWhereAsync<StoredObject>(new { AccountId, Name = oldName });
            if (obj == null) return new BadRequestObjectResult($"Object named {oldName} in account Id {AccountId} not found.");

            var ct = new ChangeTracker<StoredObject>(obj);
            obj.Name = newName;
            await Data.UpdateAsync(obj, ct);
            return new OkResult();
        }

        public async Task<IActionResult> Get(string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = AccountId, name });
            if (result == null) return new BadRequestResult();
            return new OkObjectResult(result);
        }

        public async Task<IActionResult> Exists(string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.ExistsWhereAsync<StoredObject>(new { accountId = AccountId, name });
            return new OkObjectResult(result);
        }

        public async Task<IActionResult> Delete(string name)
        {
            name = HttpUtility.UrlDecode(name);
            var result = await Data.GetWhereAsync<StoredObject>(new { accountId = AccountId, name });
            if (result == null) return new OkResult();
            await Data.DeleteAsync<StoredObject>(result.Id);
            return new OkResult();
        }

        public async Task<IActionResult> List(ListStoredObjects query)
        {
            query.AccountId = AccountId;
            var results = await query.ExecuteAsync(Data.GetConnection);
            return new OkObjectResult(results);
        }
    }
}
