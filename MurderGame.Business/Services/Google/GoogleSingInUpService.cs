using Microsoft.AspNetCore.Identity;
using MurderGame.Dtos.LoginDtos;
using MurderGame.Entities.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Business.Services.Google
{
    public class GoogleSingInUpService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoogleSingInUpService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<SignInResult> LoginAsync(LoginDto loginDto)
        {
            // Kullanıcı adı veya e-posta ile kullanıcıyı bulma
            var user = await _userManager.FindByNameAsync(loginDto.LoginIdentifier)
                       ?? await _userManager.FindByEmailAsync(loginDto.LoginIdentifier);

            if (user == null)
            {
                return SignInResult.Failed;  // Kullanıcı bulunamadı
            }

            // Kullanıcıyı parola ile doğrula
            return await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);
        }
        public async Task<ApplicationUser> HandleGoogleLoginAsync(string email, string name)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    UserNickName = name,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member");
                }
            }

            return user;
        }

    }
}

