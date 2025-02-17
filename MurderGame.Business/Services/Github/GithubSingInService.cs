using Microsoft.AspNetCore.Identity;
using MurderGame.Entities.Domains;
using System;
using System.Threading.Tasks;

namespace MurderGame.Business.Services.Github
{
    public class GitHubSignInService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GitHubSignInService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> HandleGitHubLoginAsync(string loginProvider, string providerKey)
        {
            if (string.IsNullOrEmpty(loginProvider) || string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentException("Geçersiz login provider veya provider key.");
            }

            var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
            return user;
        }
    }
}