using System.Collections.Generic;
using CloudObjects.App.Queries;
using CloudObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web;
using CloudObjects.App.Interfaces;

namespace CloudObjects.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class ObjectsController : CommonController
    {
        private readonly IStoredObjectService _storedObjectService;

        public ObjectsController(
            HttpContext httpContext,
            IStoredObjectService storedObjectService) : base(httpContext)
        {
            _storedObjectService = storedObjectService;
        }

        [HttpPost]
        public async Task<ActionResult<StoredObject>> Post(StoredObject @object)
        {
            @object.AccountId = AccountId;
            @object.Length = @object.Json.Length;

            await _storedObjectService.CreateAsync(@object);

            return CreatedAtAction(nameof(Get), new {name = @object.Name}, @object);
        }

        [HttpPut]
        public async Task<ActionResult<StoredObject>> Put(StoredObject @object)
        {
            @object.AccountId = AccountId;
            @object.Length = @object.Json.Length;

            return await _storedObjectService.ReplaceAsync(@object);
        }

        [HttpPut]
        [Route("Rename")]
        public async Task<IActionResult> Put([FromQuery]string oldName, [FromQuery]string newName)
        {
            oldName = HttpUtility.UrlDecode(oldName);
            newName = HttpUtility.UrlDecode(newName);

            await _storedObjectService.RenameAsync(AccountId, oldName, newName);  
            return NoContent();
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<ActionResult<StoredObject>> Get([FromRoute]string name)
        {
            name = HttpUtility.UrlDecode(name);
            return await _storedObjectService.GetAsync(AccountId, name);
        }

        [HttpGet]
        [Route("Exists/{name}")]
        public async Task<ActionResult<bool>> Exists([FromRoute]string name)
        {
            name = HttpUtility.UrlDecode(name);
            return await _storedObjectService.ExistsAsync(AccountId, name);
        }

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            name = HttpUtility.UrlDecode(name);
            await _storedObjectService.DeleteAsync(AccountId, name);
            return NoContent();
        }

        [HttpDelete]
        [Route("All")]
        public async Task<IActionResult> DeleteAll()
        {
            await _storedObjectService.DeleteAsync(AccountId);
            return NoContent();
        }

        [HttpPost]
        [Route("List")]
        public async Task<ActionResult<List<StoredObject>>> List(ListStoredObjects query)
        {
            return await _storedObjectService.ListAsync(AccountId, query);
        }
    }
}
