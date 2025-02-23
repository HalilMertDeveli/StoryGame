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
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Home");
            }

            // Veritabanından entity'yi alın
            var userProfileEntity = await _context.UserProfileDetailsTable.FirstOrDefaultAsync(x => x.UserId == user.Id);

            // Entity'yi DTO'ya dönüştürün
            UserProfileDto model = new UserProfileDto();
            if (userProfileEntity != null)
            {
                model.DisplayName = userProfileEntity.DisplayName;
                model.DateOfBirth = userProfileEntity.DateOfBirth;
                model.Location = userProfileEntity.Location;
                model.PhoneNumber = userProfileEntity.PhoneNumber;
                // Eğer DTO'nuzda ek alanlar varsa burada atayabilirsiniz.
            }
            // Eğer entity bulunamazsa, boş bir DTO gönderilir.

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileDto model)
        {
            // Öncelikle gelen modelin ASP.NET Core ModelState doğrulaması varsa kontrol edin
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Home");
            }

            // Servis aracılığıyla FluentValidation uygulanıyor
            var result = await _profileDetailServiceInstance.HandleProfileUpdateAsync(model, user);

            if (!result.Success)
            {
                // Validasyon hatalarını ModelState'e ekle
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model); // Hata varsa, view'u model ile birlikte tekrar render et
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Profile", "Member");
        }

        [HttpGet]
        public IActionResult Payment()
        {
            return View();
        }
    }
}
