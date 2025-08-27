using System;
using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{
public class Match
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int User1Id { get; set; } // Kullanıcı 1
    public User User1 { get; set; }

    [Required]
    public int User2Id { get; set; } // Kullanıcı 2
    public User User2 { get; set; }

    public DateTime MatchedAt { get; set; } = DateTime.UtcNow; // Eşleşme zamanı

    public bool IsAcceptedByUser1 { get; set; } = false; // Kullanıcı 1 onayı
    public bool IsAcceptedByUser2 { get; set; } = false; // Kullanıcı 2 onayı
}

}
