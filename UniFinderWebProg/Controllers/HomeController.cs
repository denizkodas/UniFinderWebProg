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

        // Kullan�c� profil listesi sayfas�
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
                                        .Include(u => u.UserPhotos) // Kullan�c� foto�raflar�n� dahil ediyoruz
                                        .Where(u => u.Id != currentUserId // Kendisi de�il
                                                    && !likedUserIds.Contains(u.Id) // Daha �nce be�enilmemi�
                                                    && u.Gender == currentUser.InterestedIn) // �lgi alan�na uygun
                                        .ToList();

            if (!usersToDisplay.Any())
            {
                ViewBag.NoUsersLeft = true; // E�er be�enilecek ba�ka kullan�c� yoksa model null g�nderiyoruz
                return View("ProfileList", null);  
            }

            // Rastgele bir kullan�c�y� se�iyoruz
            var nextUser = usersToDisplay.OrderBy(u => Guid.NewGuid()).FirstOrDefault();
            ViewBag.LikerId = currentUserId;

            // ProfileList view'�n� g�steriyoruz
            return View(nextUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HandleLikeDislike(int likedId, bool isLike)
        {
            int? likerId = HttpContext.Session.GetInt32("UserId");
            if (likerId == null)
            {
                TempData["ErrorMessage"] = "L�tfen giri� yap�n.";
                return RedirectToAction("Login", "Account");
            }

            if (likedId == 0 || likerId == 0)
            {
                return BadRequest("Ge�ersiz i�lem.");
            }

            if (isLike)
            {
                // Kullan�c� zaten be�endiyse, yeni bir be�eni kaydetme
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

                // Mevcut e�le�meyi kontrol et
                var existingMatch = _context.Matches.FirstOrDefault(m =>
                    (m.User1Id == likerId && m.User2Id == likedId) ||
                    (m.User1Id == likedId && m.User2Id == likerId));

                if (existingMatch != null)
                {
                    // E�le�me varsa, kullan�c�n�n onay�n� g�ncelle
                    if (existingMatch.User1Id == likerId)
                    {
                        existingMatch.IsAcceptedByUser1 = true;
                    }
                    else if (existingMatch.User2Id == likerId)
                    {
                        existingMatch.IsAcceptedByUser2 = true;
                    }

                    // E�er her iki kullan�c� da e�le�meyi onaylad�ysa
                    if (existingMatch.IsAcceptedByUser1 && existingMatch.IsAcceptedByUser2)
                    {
                        existingMatch.MatchedAt = DateTime.UtcNow;
                        TempData["SuccessMessage"] = "Yeni bir e�le�me olu�tu!";

                        // E�le�me say�s�n� her iki kullan�c� i�in art�rma
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
                    // E�le�me yoksa yeni e�le�me olu�tur
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
                TempData["ErrorMessage"] = "L�tfen giri� yap�n.";
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _context.Users
                .Include(u => u.UserPhotos)  
                .FirstOrDefault(u => u.Id == userId);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Kullan�c� bulunamad�.";
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
                TempData["ErrorMessage"] = "L�tfen giri� yap�n.";
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _context.Users
                .Include(u => u.UserPhotos)  
                .FirstOrDefault(u => u.Id == userId);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Kullan�c� bulunamad�.";
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
                TempData["ErrorMessage"] = "L�tfen giri� yap�n.";
                return RedirectToAction("Login", "Account");
            }

            var currentUser = _context.Users.Include(u => u.UserPhotos).FirstOrDefault(u => u.Id == userId);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Kullan�c� bulunamad�.";
                return RedirectToAction("Login", "Account");
            }

            // Kullan�c� bilgilerini g�ncelle
            currentUser.Bio = bio;
            currentUser.Email = email;
            currentUser.University = university;
            currentUser.Location = location;

            // Foto�raflar� g�ncelle
            try
            {
                // Mevcut foto�raflar� kontrol et ve temizle (iste�e ba�l�)
                foreach (var userPhoto in currentUser.UserPhotos.ToList())
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", userPhoto.PhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); 
                    }
                    _context.UserPhotos.Remove(userPhoto); // Veritaban�ndan kald�r
                }

                // Yeni foto�raflar� kaydet
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
                TempData["ErrorMessage"] = $"Foto�raflar g�ncellenirken hata olu�tu: {ex.Message}";
                return View(currentUser);
            }

            // Kullan�c� bilgilerini kaydet
            try
            {
                _context.Update(currentUser);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bilgiler g�ncellenirken hata olu�tu: {ex.Message}";
                return View(currentUser);
            }

            TempData["SuccessMessage"] = "Bilgiler ba�ar�yla g�ncellendi!";
            return RedirectToAction("KendiProfilEdit");
        }

        public IActionResult ProfileInfo(int userId)
        {
            // �lgili kullan�c�y� veritaban�ndan �ekiyoruz
            var user = _context.Users
                               .Include(u => u.UserPhotos) 
                               .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullan�c� bulunamad�.";
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
                return Json(new { success = false, message = "Kullan�c� oturumu ge�ersiz." });
            }

            var match = _context.Matches
                                 .FirstOrDefault(m => (m.User1Id == userId || m.User2Id == userId)
                                                   && m.IsAcceptedByUser1
                                                   && m.IsAcceptedByUser2);

            if (match != null)
            {
                return Json(new { success = true, message = "Yeni bir e�le�me var!", isMatch = true });
            }

            return Json(new { success = false, message = "E�le�me yok.", isMatch = false });
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

            // Kullan�c�ya ait e�le�me say�s�n� hesaplama
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
                TempData["ErrorMessage"] = "L�tfen giri� yap�n.";
                return RedirectToAction("Login", "Account");
            }

            // Kullan�c�y� ve e�le�me durumunu kontrol et
            var matchedUser = _context.Users
                                     .Include(u => u.UserPhotos)
                                     .FirstOrDefault(u => u.Id == userId);

            if (matchedUser == null)
            {
                TempData["ErrorMessage"] = "Kullan�c� bulunamad�.";
                return RedirectToAction("Match");
            }

            // E�le�me do�rulama
            var matchExists = _context.Matches.Any(m =>
                (m.User1Id == currentUserId && m.User2Id == userId) ||
                (m.User1Id == userId && m.User2Id == currentUserId) &&
                m.IsAcceptedByUser1 && m.IsAcceptedByUser2);

            if (!matchExists)
            {
                TempData["ErrorMessage"] = "Bu kullan�c�yla e�le�meniz yok.";
                return RedirectToAction("Match");
            }

            // Mesajlar� �ekme
            var messages = _context.Message
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                            (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToList();

            // Foto�raf yollar�n� almak
            var photoPaths = matchedUser.UserPhotos?.Select(p => p.PhotoPath ?? string.Empty).ToList() ?? new List<string>();

            // Kullan�c� bilgilerini ve mesajlar� birlikte g�nder
            ViewBag.MatchedUser = matchedUser;
            ViewBag.Messages = messages;
            ViewBag.PhotoPaths = photoPaths;  // Foto�raf yollar�n� View'a g�nder
            ViewBag.CurrentUserId = currentUserId;  // currentUserId'yi ViewBag'e ekle

            return View();
        }

        // Mesaj g�nderme
        [HttpPost]
        public IActionResult SendMessage(int receiverId, string content)
        {
            int senderId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();

            if (senderId == 0 || receiverId == 0 || string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Ge�ersiz mesaj.";
                return RedirectToAction("Chat", new { userId = receiverId });
            }

            // Yeni mesaj olu�turma
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            // Mesaj� veritaban�na ekleme
            _context.Message.Add(message);
            _context.SaveChanges();

            // Mesaj g�nderildikten sonra, kullan�c�y� tekrar mesajla�ma ekran�na y�nlendirme
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

