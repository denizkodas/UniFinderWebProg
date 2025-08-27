using Microsoft.AspNetCore.Mvc;
using UniFinderWebProg.Models;
using System.Linq;
using UniFinderWebProg.Utulity;

public class LikeController : Controller
{
    private readonly UygulamaDbContext _context;

    public LikeController(UygulamaDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult ProcessLikeDislike([FromBody] LikeDislikeModel model)
    {
        if (ModelState.IsValid)
        {
            // Like işlemi
            if (model.isLike)
            {
                var like = new Like
                {
                    LikerId = model.likerId,
                    LikedId = model.likedId,
                    LikedAt = DateTime.UtcNow
                };
                _context.Likes.Add(like);
                _context.SaveChanges();
            }
            // Dislike işlemi
            else
            {
                var dislike = new Like
                {
                    LikerId = model.likerId,
                    LikedId = model.likedId,
                    LikedAt = DateTime.UtcNow
                };
                _context.Likes.Remove(dislike);
                _context.SaveChanges();
            }

            // Yeni kullanıcıyı almak için
            var nextUser = _context.Users
                                   .Where(u => u.Id != model.likedId)
                                   .OrderBy(u => Guid.NewGuid())  // Rastgele bir kullanıcı seç
                                   .FirstOrDefault();

            // Yeni kullanıcının bilgilerini JSON olarak döndür
            if (nextUser != null)
            {
                return Json(new
                {
                    success = true,
                    nextUser = new
                    {
                        username = nextUser.Username,
                        age = DateTime.Now.Year - nextUser.BirthDate.Year, 
                        profileImageUrl = "https://via.placeholder.com/300x400" 
                    }
                });
            }

            return Json(new { success = false });
        }

        return Json(new { success = false });
    }
}

public class LikeDislikeModel
{
    public bool isLike { get; set; }
    public int likerId { get; set; }
    public int likedId { get; set; }
}
