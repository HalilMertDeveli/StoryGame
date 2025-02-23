using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MurderGame.DataAccess.Context;
using MurderGame.Dtos.UserDtos;
using MurderGame.Entities.Domains;
using FluentValidation;
using FluentValidation.Results;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;

namespace MurderGame.Business.Services
{
    public class ProfileDetailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IValidator<UserProfileDto> _validator;

        public ProfileDetailService(UserManager<ApplicationUser> userManager, AppDbContext context, IValidator<UserProfileDto> validator)
        {
            _userManager = userManager;
            _context = context;
            _validator = validator;
        }

        public async Task<(bool Success, string Message)> HandleProfileUpdateAsync(UserProfileDto model, ApplicationUser user)
        {
            // FluentValidation ile modeli doğrula
            ValidationResult validationResult = _validator.Validate(model);
            if (!validationResult.IsValid)
            {
                // Hata mesajlarını birleştir ve döndür
                string errorMessage = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errorMessage);
            }

            // Kullanıcının profil bilgisi var mı kontrol et
            var existingProfile = await _context.UserProfileDetailsTable.FindAsync(user.Id);

            if (existingProfile == null)
            {
                // Profil yoksa yeni kayıt oluştur
                var userProfile = new ApplicationUserProfileDetails
                {
                    UserId = user.Id,
                    DisplayName = model.DisplayName,
                    DateOfBirth = model.DateOfBirth,
                    Location = model.Location,
                    PhoneNumber = model.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserProfileDetailsTable.Add(userProfile);
            }
            else
            {
                // Profil varsa güncelleme yap
                existingProfile.DisplayName = model.DisplayName;
                existingProfile.DateOfBirth = model.DateOfBirth;
                existingProfile.Location = model.Location;
                existingProfile.PhoneNumber = model.PhoneNumber;
                existingProfile.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return (true, "Profil başarıyla güncellendi!");
        }
    }
}
