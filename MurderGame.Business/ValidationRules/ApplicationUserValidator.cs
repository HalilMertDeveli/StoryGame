using FluentValidation;
using MurderGame.Entities.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Business.ValidationRules
{
    public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
    {
        public ApplicationUserValidator()
        {
            // Ad Doğrulama
            RuleFor(x => x.UserNickName)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
                .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.");

            // E-posta Doğrulama
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            // Şifre Doğrulama
            RuleFor(x => x.PasswordHash)
                .NotEmpty().WithMessage("Şifre zorunludur.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");

            // Bio (Opsiyonel ama uzunluk kontrolü)
            RuleFor(x => x.Bio)
                .MaximumLength(250).WithMessage("Biyografi 250 karakterden uzun olamaz.");

            // Profil Resmi (Opsiyonel ama kontrol)
            RuleFor(x => x.ProfilePicture)
                .Must(IsValidImageFormat).WithMessage("Geçerli bir resim formatı olmalıdır (jpg, png, jpeg).")
                .When(x => !string.IsNullOrEmpty(x.ProfilePicture));
            RuleFor(x => x.ApplicationUserProfileDetails).Null();


        }

        private bool IsValidImageFormat(string profilePicture)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            return allowedExtensions.Any(ext => profilePicture.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
