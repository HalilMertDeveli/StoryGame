using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MurderGame.Business.Services;
using MurderGame.Dtos.SingupDtos;
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
        public IActionResult SingUp(SignUpDto signUpDto)
        {
            var validationResult = _signUpService.Validate(signUpDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(signUpDto);
            }

            // DTO’dan ApplicationUser oluşturma
            var applicationUser = new ApplicationUser
            {
                UserNickName = signUpDto.UserNickName,
                Email = signUpDto.Email,
                PasswordHash = signUpDto.PasswordHash,
                Bio = signUpDto.Bio
            };

            // Profil resmini kaydetme
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var filePath = Path.Combine(uploadsPath, signUpDto.ProfilePicture.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                signUpDto.ProfilePicture.CopyTo(stream);
            }
            applicationUser.ProfilePicture = "/uploads/" + signUpDto.ProfilePicture.FileName;

            // Burada veritabanı kaydı yapılabilir
            TempData["Message"] = "Kayıt başarılı!";
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult SingUp()
        {
            return View(new SignUpDto());
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
