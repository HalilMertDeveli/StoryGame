using System;

namespace MurderGame.Dtos.UserDtos
{
    public class UserProfileDto
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Location { get; set; }
    }
}