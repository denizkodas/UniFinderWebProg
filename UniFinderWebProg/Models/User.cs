using System;
using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{

public class User
{
        [Key]
        public int Id { get; set; }

        
        [MaxLength(100)]
        public string Username { get; set; }

       
        [EmailAddress]
        public string Email { get; set; }       

        [MaxLength(100)]
        public string University { get; set; }

        
        public DateTime BirthDate { get; set; }
        public string Bio { get; set; } // Kullanıcı hakkında kısa açıklama
                                        
       
        public string Gender { get; set; } // "Male", "Female", "Other"

        
        public string InterestedIn { get; set; } // "Male", "Female", "Everyone"

        public string Location { get; set; } // Şehir veya coğrafi bölge

        
        public string Password { get; set; }
        public virtual ICollection<PasswordReset>? PasswordResets { get; set; }
        public ICollection<UserPhoto>? UserPhotos { get; set; }
        public int? MatchCount { get; set; } = 0;  // Varsayılan olarak 0
                                                   // Kullanıcıya ait eşleşmeler (User1 olarak)
        public ICollection<Match>? MatchesAsUser1 { get; set; }

        // Kullanıcıya ait eşleşmeler (User2 olarak)
        public ICollection<Match>? MatchesAsUser2 { get; set; }


    }


}