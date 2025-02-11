using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MurderGame.Business.Services;
using MurderGame.Entities.Domains;
using MurderGame.Models;

namespace MurderGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignUpService _signUpService;

        public HomeController(ILogger<HomeController> logger, SignUpService signUpService)
        {
            _logger = logger;
            _signUpService = signUpService;
        }

        [HttpPost]
        public IActionResult SingUp(ApplicationUser applicationUser, IFormFile ProfilePicture)
        {
            var validationResult = _signUpService.Validate(applicationUser);

            // FluentValidation hatalarını ModelState'e ekle
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(applicationUser);  // Model eksiksiz geri döndürülüyor
            }


            // Dosya yükleme işlemi
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", ProfilePicture.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(stream);
                }
                applicationUser.ProfilePicture = "/uploads/" + ProfilePicture.FileName;
            }

            TempData["Message"] = "Kayıt başarılı!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult SingUp()
        {
            return View(new ApplicationUser());
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
