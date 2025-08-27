using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }  // Şifre sıfırlama token'ı, URL'den alınacak

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Parola en az 6 karakter uzunluğunda olmalıdır.")]
        public string NewPassword { get; set; }  // Yeni şifre

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }  // Yeni şifrenin teyidi
    }
}
