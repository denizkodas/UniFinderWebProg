using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{
    public class PasswordReset
    {
        [Key]
        public int Id { get; set; } // Her bir şifre sıfırlama işlemi için benzersiz ID

        public string? ResetPasswordToken { get; set; } // Şifre sıfırlama token'ı

        public DateTime? TokenExpiration { get; set; } // Token'ın geçerlilik süresi

        public DateTime? CreatedAt { get; set; } // Token oluşturulma tarihi

        public int UserId { get; set; } // Kullanıcıya ilişkin referans

        public User User { get; set; } // Kullanıcı ile ilişkilendirme

        public bool IsUsed { get; set; } // Token'ın kullanılıp kullanılmadığı
    }
}
