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

            // 🔹 Kullanıcıdan her seferinde izin almayı zorunlu kıl
            properties.Items["prompt"] = "consent";  // Kullanıcıdan her defasında onay alınır.
            properties.Items["scope"] = "user:email"; // Email adresini zorunlu alır.
            properties.Items["force_verify"] = "true";
            return Challenge(properties, "GitHub");
        }


        [HttpGet]
        public async Task<IActionResult> GitHubSignUpCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "GitHub bilgileri alınamadı. Lütfen tekrar deneyin.";
                return RedirectToAction("SingUp");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email bilgisi alınamadı.";
                return RedirectToAction("SingUp");
            }

            // 🔹 Kullanıcıyı kontrol et ve eğer varsa hata döndür
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Bu email adresi zaten kayıtlı. Lütfen giriş yapın.";
                return RedirectToAction("Login");
            }

            // 🔹 Yeni kullanıcı oluştur ve kayıt işlemini gerçekleştir
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                UserNickName = name,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Kullanıcı kaydedilemedi.";
                return RedirectToAction("SingUp");
            }

            // Kullanıcıya "Member" rolünü ata
            await _userManager.AddToRoleAsync(user, "Member");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Member");
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

            // Kullanıcıyı loginProvider ve providerKey ile kontrol et
            var user = await _gitHubSignInService.HandleGitHubLoginAsync(loginProvider, providerKey);

            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Member");
            }

            // Kullanıcı bulunamadı, yeni bir kayıt oluşturmayı dene
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "GitHubUser";

            if (string.IsNullOrEmpty(email))
            {
                email = $"{name}@github.com";  // Geçici email
            }

            try
            {
                user = await _gitHubSignUpService.HandleGitHubSignUpAsync(email, name);

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


        // ---------- Other Actions ----------
        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

  public HomeController(ILogger<HomeController> logger, SignUpService signUpService,
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager, GoogleSingInUpService loginService, FacebookSignUpService facebookSignUpService, TwitterSignUpService twitterSignUpService)
  {
      _logger = logger;
      _signUpService = signUpService;
      _userManager = userManager;
      _signInManager = signInManager;
      _loginService = loginService;
      _facebookSignUpService = facebookSignUpService;
      _twitterSignUpService = twitterSignUpService;
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

  [HttpGet]
  public IActionResult Login()
  {
      return View();
  }
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
      {
          return RedirectToAction("Index", "Member");
      }

      ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı. Lütfen tekrar deneyin.");
      return View(loginDto);
  }

  [HttpGet]
  public IActionResult FacebookSignUp()
  {
      var redirectUrl = Url.Action("FacebookCallback", "Home");
      var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
      return Challenge(properties, "Facebook");
  }

  [HttpGet]
  public async Task<IActionResult> FacebookCallback()
  {
      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null) return RedirectToAction("Login");

      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      var name = info.Principal.FindFirstValue(ClaimTypes.Name);

      // Kullanıcıyı kontrol et ve yoksa kaydet
      var user = await _facebookSignUpService.HandleFacebookSignUpAsync(email, name);

      // Kullanıcıyı giriş yaptır
      await _signInManager.SignInAsync(user, isPersistent: false);
      return RedirectToAction("Index", "Member");
  }

  [HttpGet]
  public IActionResult FacebookLogin()
  {
      var redirectUrl = Url.Action("FacebookLoginCallback", "Home");
      var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
      return Challenge(properties, "Facebook");
  }

  [HttpGet]
  public async Task<IActionResult> FacebookLoginCallback()
  {
      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null) return RedirectToAction("Login");

      // info.LoginProvider ve info.ProviderKey'i al ve metodu doğru şekilde çağır
      var user = await _facebookSignUpService.HandleFacebookLoginAsync(info.LoginProvider, info.ProviderKey);

      if (user != null)
      {
          await _signInManager.SignInAsync(user, isPersistent: false);
          return RedirectToAction("Index", "Member");
      }

      return RedirectToAction("Login");
  }

  [HttpGet]
  public IActionResult TwitterLogin()
  {
      var redirectUrl = Url.Action("TwitterCallback", "Home");
      var properties = _signInManager.ConfigureExternalAuthenticationProperties("Twitter", redirectUrl);
      return Challenge(properties, "Twitter");
  }

  [HttpGet]
  public async Task<IActionResult> TwitterCallback()
  {
      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null) return RedirectToAction("Login");

      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      var name = info.Principal.FindFirstValue(ClaimTypes.Name);

      // Kullanıcıyı kontrol et ve yoksa kaydet
      var user = await _twitterSignUpService.HandleTwitterSignUpAsync(email, name);

      // Kullanıcıyı giriş yaptır
      await _signInManager.SignInAsync(user, isPersistent: false);
      return RedirectToAction("Index", "Member");
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
  //Google operations
  [HttpGet]
  public IActionResult GoogleLogin()
  {
      var redirectUrl = Url.Action("GoogleCallback", "Home");
      var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
      return Challenge(properties, "Google");
  }


  [HttpGet]
  public async Task<IActionResult> GoogleCallback()
  {
      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null) return RedirectToAction("Login");

      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      var name = info.Principal.FindFirstValue(ClaimTypes.Name);

      // Kullanıcıyı kontrol et ve yoksa kaydet
      var user = await _loginService.HandleGoogleLoginAsync(email, name);

      // Kullanıcıyı giriş yaptır
      await _signInManager.SignInAsync(user, isPersistent: false);
      return RedirectToAction("Index", "Member");
  }

  [HttpGet]
  public IActionResult GoogleSignUp()
  {
      var redirectUrl = Url.Action("GoogleSignUpCallback", "Home");
      var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
      return Challenge(properties, "Google");
  }

  [HttpGet]
  public async Task<IActionResult> GoogleSignUpCallback()
  {
      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null) return RedirectToAction("SingUp");

      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      var name = info.Principal.FindFirstValue(ClaimTypes.Name);

      // Kullanıcı mevcut mu?
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
          // Yeni kullanıcı oluştur
          user = new ApplicationUser
          {
              UserName = email,
              Email = email,
              UserNickName = name,
              CreatedAt = DateTime.UtcNow
          };

          var result = await _userManager.CreateAsync(user);
          if (result.Succeeded)
          {
              // Kullanıcıya "Member" rolünü ata
              await _userManager.AddToRoleAsync(user, "Member");
              // Google girişini ilişkilendir
              await _userManager.AddLoginAsync(user, info);
          }
          else
          {
              // Hata mesajlarını ekle
              foreach (var error in result.Errors)
              {
                  ModelState.AddModelError(string.Empty, error.Description);
              }
              return RedirectToAction("SingUp");
          }
      }

      // Kullanıcıyı giriş yaptır
      await _signInManager.SignInAsync(user, isPersistent: false);
      return RedirectToAction("Index", "Member");
  }
        
    }
}
