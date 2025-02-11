
using FluentValidation;
using FluentValidation.Results;
using MurderGame.Entities.Domains;
using Microsoft.AspNetCore.Http;

namespace MurderGame.Business.Services
{
    public class SignUpService
    {
        private readonly IValidator<ApplicationUser> _validator;

        public SignUpService(IValidator<ApplicationUser> validator)
        {
            _validator = validator;
        }

        public ValidationResult Validate(ApplicationUser applicationUser, IFormFile profilePicture)
        {
            var validationResult = _validator.Validate(applicationUser);

            // Resim yüklenmiş mi kontrol et
            if (profilePicture == null || profilePicture.Length == 0)
            {
                validationResult.Errors.Add(new ValidationFailure("ProfilePicture", "Profil resmi yüklemek zorunludur."));
            }

            return validationResult;
        }
    }
}

