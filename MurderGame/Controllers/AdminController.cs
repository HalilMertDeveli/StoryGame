using Microsoft.AspNetCore.Mvc;

namespace MurderGame.UI.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
