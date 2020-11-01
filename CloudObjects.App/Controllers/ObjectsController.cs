using CloudObjects.App.Queries;
using CloudObjects.Models;
using CloudObjects.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CloudObjects.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class ObjectsController : ControllerBase
    {
        private readonly ObjectsService _objectsService;

        public ObjectsController(ObjectsService objectsService)
        {
            _objectsService = objectsService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(StoredObject @object) => await _objectsService.Post(@object);

        [HttpPut]
        public async Task<IActionResult> Put(StoredObject @object) => await _objectsService.Put(@object);

        [HttpPut]
        [Route("rename")]
        public async Task<IActionResult> Put([FromQuery] string oldName, [FromQuery] string newName) => await _objectsService.Rename(oldName, newName);

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> Get([FromRoute] string name) => await _objectsService.Get(name);

        [HttpGet]
        [Route("exists/{name}")]
        public async Task<IActionResult> Exists([FromRoute] string name) => await _objectsService.Exists(name);

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> Delete(string name) => await _objectsService.Delete(name);

        [HttpPost]
        [Route("list")]
        public async Task<IActionResult> List(ListStoredObjects query) => await _objectsService.List(query);
    }
}
