using System;
using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{
public class ProfilePreferences
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } // İlişkilendirme

    public int MinAge { get; set; } // Minimum yaş tercihi

    public int MaxAge { get; set; } // Maksimum yaş tercihi

    public string PreferredGender { get; set; } // "Male", "Female", "Everyone"

    public string PreferredLocation { get; set; } // Tercih edilen şehir
}

}