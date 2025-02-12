using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Dtos.LoginDtos
{
    public class LoginDto
    {
        public string LoginIdentifier { get; set; }  // Kullanıcı adı veya E-posta
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

