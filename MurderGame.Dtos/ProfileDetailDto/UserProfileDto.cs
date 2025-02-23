using System;

namespace MurderGame.Dtos.UserDtos
{
    public class UserProfileDto
    {
        // Kullanıcı adı (DisplayName) - zorunlu
        public string DisplayName { get; set; }

        // Doğum tarihi (DateOfBirth) - zorunlu
        public DateTime DateOfBirth { get; set; }

        // Konum (Location) - isteğe bağlı, maksimum 100 karakter
        public string? Location { get; set; }

        // Telefon numarası (PhoneNumber) - zorunlu ve belirli formata uygun olmalı
        public string PhoneNumber { get; set; }
    }
}