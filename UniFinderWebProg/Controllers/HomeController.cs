using Microsoft.AspNetCore.Mvc;
using UniFinderWebProg.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniFinderWebProg.Utulity;
using Newtonsoft.Json;
using UniFinderWebProg.VModels;


namespace UniFinderWebProg.Controllers
{
  
    public class HomeController : Controller
    {


        private readonly UygulamaDbContext _context;

        public HomeController(UygulamaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Kullanýcý profil listesi sayfasý
        [HttpGet]
        public IActionResult ProfileList()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

            var likedUserIds = _context.Likes
                                       .Where(l => l.LikerId == currentUserId)
                                       .Select(l => l.LikedId)
                                       .ToList();

            var usersToDisplay = _context.Users
                                        .Include(u => u.UserPhotos) // Kullanýcý fotoðraflarýný dahil ediyoruz
                                        .Where(u => u.Id != currentUserId // Kendisi deðil
                                                    && !likedUserIds.Contains(u.Id) // Daha önce beðenilmemiþ
                                                    && u.Gender == currentUser.InterestedIn) // Ýlgi alanýna uygun
                                        .ToList();

            if (!usersToDisplay.Any())
            {
                ViewBag.NoUsersLeft = true; // Eðer beðenilecek baþka kullanýcý yoksa model null gönderiyoruz
                return View("ProfileList", null);  
            }

            // Rastgele bir kullanýcýyý seçiyoruz
            var nextUser = usersToDisplay.OrderBy(u => Guid.NewGuid()).FirstOrDefault();
            ViewBag.LikerId = currentUserId;

            // ProfileList view'ýný gösteriyoruz
            return View(nextUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HandleLikeDislike(int likedId, bool isLike)
        {
            int? likerId = HttpContext.Session.GetInt32("UserId");
            if (likerId == null)
            {
                TempData["ErrorMessage"] = "Lütfen giriþ yapýn.";
                return RedirectToAction("Login", "Account");
            }

            if (likedId == 0 || likerId == 0)
            {
                return BadRequest("Geçersiz iþlem.");
            }

            if (isLike)
            {
                // Kullanýcý zaten beðendiyse, yeni bir beðeni kaydetme
                if (!_context.Likes.Any(l => l.LikerId == likerId && l.LikedId == likedId))
                {
                    var like = new Like
                    {
                        LikerId = likerId.Value,
                        LikedId = likedId,
                        LikedAt = DateTime.UtcNow
                    };

                    _context.Likes.Add(like);
                    _context.SaveChanges();
                }

                // Mevcut eþleþmeyi kontrol et
                var existingMatch = _context.Matches.FirstOrDefault(m =>
                    (m.User1Id == likerId && m.User2Id == likedId) ||
                    (m.User1Id == likedId && m.User2Id == likerId));

                if (existingMatch != null)
                {
                    // Eþleþme varsa, kullanýcýnýn onayýný güncelle
                    if (existingMatch.User1Id == likerId)
                    {
                        existingMatch.IsAcceptedByUser1 = true;
                    }
                    else if (existingMatch.User2Id == likerId)
                    {
                        existingMatch.IsAcceptedByUser2 = true;
                    }

                    // Eðer her iki kullanýcý da eþleþmeyi onayladýysa
                    if (existingMatch.IsAcceptedByUser1 && existingMatch.IsAcceptedByUser2)
                    {
                        existingMatch.MatchedAt = DateTime.UtcNow;
                        TempData["SuccessMessage"] = "Yeni bir eþleþme oluþtu!";

                        // Eþleþme sayýsýný her iki kullanýcý için artýrma
                        var user1 = _context.Users.FirstOrDefault(u => u.Id == existingMatch.User1Id);
                        var user2 = _context.Users.FirstOrDefault(u => u.Id == existingMatch.User2Id);
                        if (user1 != null && user2 != null)
                        {
                            user1.MatchCount++;
                            user2.MatchCount++;
                            _context.SaveChanges();
                        }
                    }

                    _context.SaveChanges();
                }
                else
                {
                    // Eþleþme yoksa yeni eþleþme oluþtur
                    var match = new Match
                    {
                        User1Id = likerId.Value,
                        User2Id = likedId,
                        MatchedAt = DateTime.UtcNow,
                        IsAcceptedByUser1 = true,
                        IsAcceptedByUser2 = false
                    };

                    _context.Matches.Add(match);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("ProfileList");
        }




        [HttpGet]
        public IActionResult KendiProfil()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Lütfen giriþ yapýn.";
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _context.Users
                .Include(u => u.UserPhotos)  
                .FirstOrDefault(u => u.Id == userId);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Kullanýcý bulunamadý.";
                return RedirectToAction("Login", "Account");
            }

            
            return View(currentUser);
        }


        [HttpGet]
        public IActionResult KendiProfilEdit()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Lütfen giriþ yapýn.";
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _context.Users
                .Include(u => u.UserPhotos)  
                .FirstOrDefault(u => u.Id == userId);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Kullanýcý bulunamadý.";
                return RedirectToAction("Login", "Account");
            }

           
            return View(currentUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KendiProfilEdit(IFormFile[] photos, string bio, string email, string university, string location)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Lütfen giriþ yapýn.";
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _context.Users.Include(u => u.UserPhotos).FirstOrDefault(u => u.Id == userId);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Kullanýcý bulunamadý.";
                return RedirectToAction("Login", "Account");
            }

            // Kullanýcý bilgilerini güncelle
            currentUser.Bio = bio;
            currentUser.Email = email;
            currentUser.University = university;
            currentUser.Location = location;

            // Fotoðraflarý güncelle
            try
            {
                // Mevcut fotoðraflarý kontrol et ve temizle (isteðe baðlý)
                foreach (var userPhoto in currentUser.UserPhotos.ToList())
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", userPhoto.PhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); 
                    }
                    _context.UserPhotos.Remove(userPhoto); // Veritabanýndan kaldýr
                }

                // Yeni fotoðraflarý kaydet
                foreach (var photo in photos)
                {
                    if (photo != null && photo.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            photo.CopyTo(stream);
                        }

                        var userPhoto = new UserPhoto
                        {
                            UserId = currentUser.Id,
                            PhotoPath = "/images/" + fileName
                        };

                        _context.UserPhotos.Add(userPhoto);
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fotoðraflar güncellenirken hata oluþtu: {ex.Message}";
                return View(currentUser);
            }

            // Kullanýcý bilgilerini kaydet
            try
            {
                _context.Update(currentUser);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bilgiler güncellenirken hata oluþtu: {ex.Message}";
                return View(currentUser);
            }

            TempData["SuccessMessage"] = "Bilgiler baþarýyla güncellendi!";
            return RedirectToAction("KendiProfilEdit");
        }

        public IActionResult ProfileInfo(int userId)
        {
            // Ýlgili kullanýcýyý veritabanýndan çekiyoruz
            var user = _context.Users
                               .Include(u => u.UserPhotos) 
                               .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanýcý bulunamadý.";
                return RedirectToAction("ProfileList"); 
            }

            
            return View(user);
        }
        [HttpGet]
        [HttpPost]
        [Route("home/match")]
        public IActionResult Match()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var matches = _context.Matches
                .Where(m => (m.User1Id == userId || m.User2Id == userId) &&
                            m.IsAcceptedByUser1 && m.IsAcceptedByUser2)
                .ToList();

            var matchUsers = matches.Select(match =>
            {
                int matchedUserId = match.User1Id == userId ? match.User2Id : match.User1Id;
                return _context.Users
                    .Include(u => u.UserPhotos)
                    .FirstOrDefault(u => u.Id == matchedUserId);
            }).Where(u => u != null).ToList();

            return View(matchUsers);
        }



        [Route("home/checkmatchstatus")]
        public JsonResult CheckMatchStatus()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Kullanýcý oturumu geçersiz." });
            }

            var match = _context.Matches
                                 .FirstOrDefault(m => (m.User1Id == userId || m.User2Id == userId)
                                                   && m.IsAcceptedByUser1
                                                   && m.IsAcceptedByUser2);

            if (match != null)
            {
                return Json(new { success = true, message = "Yeni bir eþleþme var!", isMatch = true });
            }

            return Json(new { success = false, message = "Eþleþme yok.", isMatch = false });
        }

        
        [HttpGet]
        [Route("home/getmatchcount")]
        public IActionResult GetMatchCount()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, matchCount = 0 });
            }

            // Kullanýcýya ait eþleþme sayýsýný hesaplama
            var matchCount = _context.Matches.Count(m =>
                (m.User1Id == userId || m.User2Id == userId) &&
                m.IsAcceptedByUser1 && m.IsAcceptedByUser2);

            return Json(new { success = true, matchCount = matchCount });
        }

        public IActionResult Chat(int userId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Lütfen giriþ yapýn.";
                return RedirectToAction("Login", "Account");
            }

            // Kullanýcýyý ve eþleþme durumunu kontrol et
            var matchedUser = _context.Users
                                     .Include(u => u.UserPhotos)
                                     .FirstOrDefault(u => u.Id == userId);

            if (matchedUser == null)
            {
                TempData["ErrorMessage"] = "Kullanýcý bulunamadý.";
                return RedirectToAction("Match");
            }

            // Eþleþme doðrulama
            var matchExists = _context.Matches.Any(m =>
                (m.User1Id == currentUserId && m.User2Id == userId) ||
                (m.User1Id == userId && m.User2Id == currentUserId) &&
                m.IsAcceptedByUser1 && m.IsAcceptedByUser2);

            if (!matchExists)
            {
                TempData["ErrorMessage"] = "Bu kullanýcýyla eþleþmeniz yok.";
                return RedirectToAction("Match");
            }

            // Mesajlarý çekme
            var messages = _context.Message
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                            (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToList();

            // Fotoðraf yollarýný almak
            var photoPaths = matchedUser.UserPhotos?.Select(p => p.PhotoPath ?? string.Empty).ToList() ?? new List<string>();

            // Kullanýcý bilgilerini ve mesajlarý birlikte gönder
            ViewBag.MatchedUser = matchedUser;
            ViewBag.Messages = messages;
            ViewBag.PhotoPaths = photoPaths;  // Fotoðraf yollarýný View'a gönder
            ViewBag.CurrentUserId = currentUserId;  // currentUserId'yi ViewBag'e ekle

            return View();
        }

        // Mesaj gönderme
        [HttpPost]
        public IActionResult SendMessage(int receiverId, string content)
        {
            int senderId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();

            if (senderId == 0 || receiverId == 0 || string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Geçersiz mesaj.";
                return RedirectToAction("Chat", new { userId = receiverId });
            }

            // Yeni mesaj oluþturma
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            // Mesajý veritabanýna ekleme
            _context.Message.Add(message);
            _context.SaveChanges();

            // Mesaj gönderildikten sonra, kullanýcýyý tekrar mesajlaþma ekranýna yönlendirme
            return RedirectToAction("Chat", new { userId = receiverId });
        }


        
        [HttpPost]
        public IActionResult MarkAsRead(int messageId)
        {
            var message = _context.Message.FirstOrDefault(m => m.Id == messageId);
            if (message != null)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }

            return RedirectToAction("Chat", new { userId = HttpContext.Session.GetInt32("UserId") });
        }

    }

}

