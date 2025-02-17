using Microsoft.AspNetCore.Identity;
using MurderGame.Entities.Domains;
using System;
using System.Threading.Tasks;

namespace MurderGame.Business.Services.Github
{
    public class GitHubSignUpService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GitHubSignUpService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> HandleGitHubSignUpAsync(string email, string username)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email bilgisi eksik. Kullanıcı kaydı gerçekleştirilemiyor.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    UserNickName = username,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Kullanıcı oluşturulamadı.");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Member");
                if (!roleResult.Succeeded)
                {
                    throw new Exception("Kullanıcıya rol atanamadı.");
                }
            }

            return user;
        }


    }
}