using Microsoft.AspNetCore.Mvc;

namespace KampMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("About")]
        public IActionResult About() => View();

        public IActionResult Resume() => View();
        public IActionResult Services() => View();
        public IActionResult Contact() => View();
    }
}