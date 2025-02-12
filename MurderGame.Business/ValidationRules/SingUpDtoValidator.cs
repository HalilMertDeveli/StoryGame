
using FluentValidation;
using MurderGame.Dtos.SingupDtos;
using MurderGame.Entities.Domains;

namespace MurderGame.Business.ValidationRules
{
    public class SingUpDtoValidator : AbstractValidator<ApplicationUser>
    {
        public class SignUpDtoValidator : AbstractValidator<SignUpDto>
        {
            public SignUpDtoValidator()
            {
                RuleFor(x => x.UserNickName)
                    .NotEmpty().WithMessage("Kullanıcı adı zorunludur.")
                    .Length(3, 50).WithMessage("Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("E-posta adresi zorunludur.")
                    .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

                RuleFor(x => x.PasswordHash)
                    .NotEmpty().WithMessage("Şifre zorunludur.")
                    .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");

                RuleFor(x => x.Bio)
                    .MaximumLength(250).WithMessage("Biyografi 250 karakterden uzun olamaz.");

                RuleFor(x => x.ProfilePicture)
                    .NotNull().WithMessage("Profil resmi yüklemek zorunludur.");
            }
        }

        private bool IsValidImageFormat(string profilePicture)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            return allowedExtensions.Any(ext => profilePicture.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
