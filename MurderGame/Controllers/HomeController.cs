﻿using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MurderGame.Business.Services;
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
        private readonly LoginService _loginService;

        public HomeController(ILogger<HomeController> logger, SignUpService signUpService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, LoginService loginService)
        {
            _logger = logger;
            _signUpService = signUpService;
            _userManager = userManager;
            _signInManager = signInManager;
            _loginService = loginService;
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
