using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MurderGame.DataAccess.Context;
using MurderGame.Dtos.UserDtos;
using MurderGame.Entities.Domains;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;
using FluentValidation;
using FluentValidation.Results;

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
            // FluentValidation ile gelen modeli doğrula
            ValidationResult validationResult = _validator.Validate(model);

            if (!validationResult.IsValid)
            {
                string errorMessage = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errorMessage);
            }

            // Kullanıcının profil bilgisi var mı kontrol et
            var existingProfile = await _context.UserProfileDetailsTable.FindAsync(user.Id);

            if (existingProfile == null)
            {
                // Eğer profil yoksa yeni bir kayıt oluştur
                var userProfile = new ApplicationUserProfileDetails
                {
                    UserId = user.Id,
                    DisplayName = model.DisplayName,
                    ProfilePicture = model.ProfilePicture,
                    Bio = model.Bio,
                    DateOfBirth = model.DateOfBirth,
                    Location = model.Location,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserProfileDetailsTable.Add(userProfile);
            }
            else
            {
                // Eğer profil zaten varsa güncelleme işlemi yap
                existingProfile.DisplayName = model.DisplayName;
                existingProfile.ProfilePicture = model.ProfilePicture;
                existingProfile.Bio = model.Bio;
                existingProfile.DateOfBirth = model.DateOfBirth;
                existingProfile.Location = model.Location;
                existingProfile.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return (true, "Profil başarıyla güncellendi!");
        }
    }
}

