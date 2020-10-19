using Microsoft.AspNetCore.Mvc;

namespace CloudObjects.App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
