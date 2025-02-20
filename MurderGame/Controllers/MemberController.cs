using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MurderGame.Business.Services;
using MurderGame.DataAccess.Context;
using MurderGame.Dtos.UserDtos;
using MurderGame.Entities.Domains;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;

namespace MurderGame.UI.Controllers
{
   

    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProfileDetailService _profileDetailServiceInstance;
        private readonly AppDbContext _context;

        public MemberController(UserManager<ApplicationUser> userManager, ProfileDetailService profileDetailServiceInstance, AppDbContext context)
        {
            _userManager = userManager;
            _profileDetailServiceInstance = profileDetailServiceInstance;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProfileDetail(UserProfileDto model)
        {
            // Kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Home");
            }

            // Servisi çağır ve sonucu kontrol et
            var result = await _profileDetailServiceInstance.HandleProfileUpdateAsync(model, user);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("ProfileDetail", "Member");
        }

        [HttpGet]
        public async Task<IActionResult> ProfileDetail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Home");
            }

            // Kullanıcının profil detaylarını veritabanından al
            var userProfile = await _context.UserProfileDetailsTable.FirstOrDefaultAsync(x => x.UserId == user.Id);

            // Eğer kayıt yoksa, yeni bir boş model oluştur
            if (userProfile == null)
            {
                userProfile = new ApplicationUserProfileDetails
                {
                    UserId = user.Id
                };
            }

            return View(userProfile); // 📌 Doğru model gönderildi
        }
        [HttpGet]
        public IActionResult Payment()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

    }

}

