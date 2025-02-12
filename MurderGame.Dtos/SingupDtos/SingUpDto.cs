using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MurderGame.Dtos.SingupDtos
{
    public class SignUpDto
    {
        public string UserNickName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // Aynı ismi kullanmak
        public string Bio { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }

}
