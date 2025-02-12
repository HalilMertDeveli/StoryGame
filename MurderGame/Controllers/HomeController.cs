using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignUpService signUpService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _signUpService = signUpService;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpPost]
        public async Task<IActionResult> SingUp(SignUpDto signUpDto)
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
                UserName = signUpDto.Email,  // UserName genelde Email ile aynı tutulur
                Bio = signUpDto.Bio
            };

            // Kullanıcı oluşturma işlemi
            var result = await _userManager.CreateAsync(applicationUser, signUpDto.PasswordHash);
            if (result.Succeeded)
            {
                // Member rolünü atama
                if (!await _userManager.IsInRoleAsync(applicationUser, "Member"))
                {
                    await _userManager.AddToRoleAsync(applicationUser, "Member");
                }

                // Kullanıcıyı giriş yaptır ve MemberController/Index'e yönlendir
                await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                return RedirectToAction("Index", "Member");
            }

            // Eğer hata varsa hata mesajlarını ekle
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(signUpDto);
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
