using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MurderGame.Business.Services.Email;
using MurderGame.Business.Services.Facebook;
using MurderGame.Business.Services.Github;
using MurderGame.Business.Services.Google;
using MurderGame.Business.Services.Twitter;
using MurderGame.Dtos.LoginDtos;
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
        private readonly GoogleSingInUpService _googleService;
        private readonly FacebookSignUpService _facebookService;
        private readonly TwitterSignUpService _twitterService;
        private readonly GitHubSignInService _gitHubSignInService;
        private readonly GitHubSignUpService _gitHubSignUpService;

        public HomeController(
            ILogger<HomeController> logger,
            SignUpService signUpService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            GoogleSingInUpService googleService,
            FacebookSignUpService facebookService,
            TwitterSignUpService twitterService,
            GitHubSignInService gitHubSignInService,
            GitHubSignUpService gitHubSignUpService)
        {
            _logger = logger;
            _signUpService = signUpService;
            _userManager = userManager;
            _signInManager = signInManager;
            _googleService = googleService;
            _facebookService = facebookService;
            _twitterService = twitterService;
            _gitHubSignInService = gitHubSignInService;
            _gitHubSignUpService = gitHubSignUpService;
        }

        // ---------- Sign Up and Login Views ----------
        [HttpGet]
        public IActionResult SingUp() => View(new SignUpDto());

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lütfen tüm alanları doldurun.");
                return View(loginDto);
            }

            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.LoginIdentifier, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

            if (loginResult.Succeeded)
                return RedirectToAction("Index", "Member");

            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı. Lütfen tekrar deneyin.");
            return View(loginDto);
        }

        // ---------- GitHub Authentication ----------
        [HttpGet]
        public IActionResult GitHubSignUp()
        {
            var redirectUrl = Url.Action("GitHubSignUpCallback", "Home");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);
            return Challenge(properties, "GitHub");
        }

        [HttpGet]
        public async Task<IActionResult> GitHubSignUpCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction("Index");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "GitHubUser";

            if (string.IsNullOrEmpty(email))
            {
                email = $"{name}@github.com";  // Geçici email
            }

            try
            {
                var user = await _gitHubSignUpService.HandleGitHubSignUpAsync(email, name);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı kaydı başarısız oldu.";
                    return RedirectToAction("Login");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Member");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult GitHubLogin()
        {
            var redirectUrl = Url.Action("GitHubLoginCallback", "Home");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);
            return Challenge(properties, "GitHub");
        }

        [HttpGet]
        public async Task<IActionResult> GitHubLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Giriş bilgileri alınamadı. Lütfen tekrar deneyin.";
                return RedirectToAction("Login");
            }

            var loginProvider = info.LoginProvider;
            var providerKey = info.ProviderKey;

            if (string.IsNullOrEmpty(loginProvider) || string.IsNullOrEmpty(providerKey))
            {
                TempData["ErrorMessage"] = "Geçersiz login provider veya provider key.";
                return RedirectToAction("Login");
            }

            var user = await _gitHubSignInService.HandleGitHubLoginAsync(loginProvider, providerKey);

            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Member");
            }

            TempData["ErrorMessage"] = "Giriş yapılamadı. Kullanıcı kaydı bulunamadı.";
            return RedirectToAction("Login");
        }

        // ---------- Other Actions ----------
        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
