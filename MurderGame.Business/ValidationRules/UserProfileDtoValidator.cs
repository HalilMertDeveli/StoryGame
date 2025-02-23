using FluentValidation;
using MurderGame.Dtos.UserDtos;
using System;

namespace MurderGame.Business.ValidationRules
{
    public class UserProfileDtoValidator : AbstractValidator<UserProfileDto>
    {
        public UserProfileDtoValidator()
        {
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olmalıdır.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Doğum tarihi boş bırakılamaz.")
                .LessThan(DateTime.UtcNow).WithMessage("Doğum tarihi gelecekte olamaz.");

            RuleFor(x => x.Location)
                .MaximumLength(100).WithMessage("Konum bilgisi en fazla 100 karakter olabilir.");

            // Telefon numarası için validation kuralı
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Geçerli bir telefon numarası giriniz.");
        }
    }
}