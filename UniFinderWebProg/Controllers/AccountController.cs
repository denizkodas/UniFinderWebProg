using Microsoft.AspNetCore.Mvc;
using UniFinderWebProg.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UniFinderWebProg.Utulity;
using System;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace UniFinderWebProg.Controllers
{
    public class AccountController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly MailService _mailService;

        public AccountController(UygulamaDbContext context, MailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }
        public IActionResult Transition()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        // Şifre sıfırlama sayfasına yönlendiren GET metodu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Şifre sıfırlama işlemini başlatan POST metodu
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Bu e-posta adresi kayıtlı değil.";
                return View();
            }

            // Şifre sıfırlama token'ı oluştur
            var resetToken = Guid.NewGuid().ToString();

            // Token bilgilerini kaydet
            var passwordReset = new PasswordReset
            {
                ResetPasswordToken = resetToken,
                TokenExpiration = DateTime.Now.AddHours(1),
                CreatedAt = DateTime.Now,
                UserId = user.Id,
                IsUsed = false
            };

            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();

            // Şifre sıfırlama bağlantısı
            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken }, Request.Scheme);
            var emailBody = $@"
                <p>Merhaba {user.Username},</p>
                <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                <p><a href='{resetLink}'>Şifre Sıfırla</a></p>
                <p>Bu bağlantı 1 saat geçerlidir.</p>";

            try
            {
                await _mailService.SendEmailAsync(user.Email, "Şifre Sıfırlama Talebi", emailBody);
                TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"E-posta gönderimi sırasında bir hata oluştu: {ex.Message}";
                return View();
            }

            return RedirectToAction("Login");
        }


        // Şifre sıfırlama sayfası
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var passwordReset = _context.PasswordResets
                .FirstOrDefault(pr => pr.ResetPasswordToken == token && pr.TokenExpiration > DateTime.Now && !pr.IsUsed);

            if (passwordReset == null)
            {
                TempData["ErrorMessage"] = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı.";
                return RedirectToAction("Login");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        // Şifre sıfırlama işlemini gerçekleştiren POST metodu
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var passwordReset = _context.PasswordResets
                .FirstOrDefault(pr => pr.ResetPasswordToken == model.Token && pr.TokenExpiration > DateTime.Now && !pr.IsUsed);

            if (passwordReset == null)
            {
                TempData["ErrorMessage"] = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı.";
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == passwordReset.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Login");
            }

            // Şifreyi hash'leyin ve veritabanına kaydedin
            user.Password = HashPassword(model.NewPassword);
            passwordReset.IsUsed = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi. Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        // Giriş sayfası
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Kullanıcı girişi işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string usernameOrEmail, string password)
        {
            // Kullanıcı adı veya e-posta ile kullanıcıyı bulma
            var user = _context.Users
                .FirstOrDefault(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            
            if (user == null)
            {
                TempData["ErrorMessage"] = "Geçersiz kullanıcı adı veya e-posta.";
                return RedirectToAction("Login");
            }

           
            if (VerifyPassword(user.Password, password)) // Şifreyi doğrulama
            {
                // Kullanıcıyı session'a kaydet
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("ProfileList", "Home");  
            }

            // Eğer şifre yanlışsa, hata mesajı
            TempData["ErrorMessage"] = "Geçersiz şifre.";
            return RedirectToAction("Login");
        }

        // Kayıt sayfası
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kullanıcı kayıt işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Şifreyi hash'le ve kullanıcıyı veritabanına ekle
                    user.Password = HashPassword(user.Password); 
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Kayıt sırasında bir hata oluştu: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurun.";
            }

            return View(user);
        }

        // Şifreyi hash'leyen fonksiyon
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Şifre doğrulama fonksiyonu
        private bool VerifyPassword(string hashedPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
   

}
