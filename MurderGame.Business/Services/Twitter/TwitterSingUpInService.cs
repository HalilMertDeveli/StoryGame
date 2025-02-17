using Microsoft.AspNetCore.Identity;
using MurderGame.Entities.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Business.Services.Twitter
{
    public class TwitterSignUpService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TwitterSignUpService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> HandleTwitterSignUpAsync(string email, string name)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
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

        public async Task<ApplicationUser> HandleTwitterLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
            return user;
        }
    }
}
