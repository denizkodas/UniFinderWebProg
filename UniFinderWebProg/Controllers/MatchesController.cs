using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniFinderWebProg.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniFinderWebProg.Utulity;

namespace UniFinderWebProg.Controllers
{
    public class MatchesController : Controller
    {
        private readonly UygulamaDbContext _context;

        public MatchesController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: Kullanıcının ilgi alanına göre profilleri getir
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Şu anki kullanıcı ID
            var currentUser = await _context.Users.FindAsync(userId);

            if (currentUser == null)
                return NotFound("Kullanıcı bulunamadı.");

            // Kullanıcının ilgi alanına göre profilleri filtrele
            var filteredUsers = _context.Users
                .Where(u => u.Id != userId) // Kendini listeleme
                .Where(u => currentUser.InterestedIn == "Everyone" || u.Gender == currentUser.InterestedIn)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    Age = (int)((System.DateTime.Now - u.BirthDate).TotalDays / 365.25), 
                    u.Gender
                })
                .ToList();

            return Json(filteredUsers); // Frontend ile veri senkronizasyonu için JSON döndür
        }

        // POST: Beğenme (Like) veya beğenmeme (Dislike)
        [HttpPost]
        public async Task<IActionResult> LikeDislike(int targetUserId, bool isLike)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Şu anki kullanıcı ID

            // Like veya Dislike işlemini kaydet
            if (isLike)
            {
                var like = new Like
                {
                    LikerId = userId,
                    LikedId = targetUserId
                };

                _context.Likes.Add(like);
                await _context.SaveChangesAsync();
            }

            return Ok("İşlem tamamlandı.");
        }
    }
}

