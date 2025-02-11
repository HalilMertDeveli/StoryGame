
using FluentValidation;
using FluentValidation.Results;
using MurderGame.Entities.Domains;

namespace MurderGame.Business.Services
{
    public class SignUpService
    {
        private readonly IValidator<ApplicationUser> _validator;

        public SignUpService(IValidator<ApplicationUser> validator)
        {
            _validator = validator;
        }

        public ValidationResult Validate(ApplicationUser applicationUser)
        {
            // FluentValidation doğrulamasını burada yapıyoruz.
            return _validator.Validate(applicationUser);
        }
    }
}

