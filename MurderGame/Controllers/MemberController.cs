using Microsoft.AspNetCore.Mvc;

namespace MurderGame.UI.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
