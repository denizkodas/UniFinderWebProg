using System;
using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{
    public class BlockedUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BlockerId { get; set; } // Engelleyen kullanıcı
        public User Blocker { get; set; }

        [Required]
        public int BlockedId { get; set; } // Engellenen kullanıcı
        public User Blocked { get; set; }

        public DateTime BlockedAt { get; set; } = DateTime.UtcNow; // Engelleme zamanı
    }
}
