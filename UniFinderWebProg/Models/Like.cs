using System;
using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LikerId { get; set; } // Beğeniyi yapan kullanıcı
        public User Liker { get; set; }

        [Required]
        public int LikedId { get; set; } // Beğenilen kullanıcı
        public User Liked { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow; // Beğenme zamanı
    }
}
