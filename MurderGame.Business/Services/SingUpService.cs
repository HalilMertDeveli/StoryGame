
using FluentValidation;
using FluentValidation.Results;
using MurderGame.Entities.Domains;
using Microsoft.AspNetCore.Http;
using MurderGame.Dtos.SingupDtos;

namespace MurderGame.Business.Services
{
    public class SignUpService
    {
        private readonly IValidator<SignUpDto> _validator;

        public SignUpService(IValidator<SignUpDto> validator)
        {
            _validator = validator;
        }

        public ValidationResult Validate(SignUpDto signUpDto)
        {
            return _validator.Validate(signUpDto);
        }
    }
}

