    using Microsoft.AspNetCore.Mvc;

namespace EpinHell.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
