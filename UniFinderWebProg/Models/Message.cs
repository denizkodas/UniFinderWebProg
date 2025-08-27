using System;
using System.ComponentModel.DataAnnotations;

namespace UniFinderWebProg.Models
{

public class Message
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SenderId { get; set; }
    public User Sender { get; set; }

    [Required]
    public int ReceiverId { get; set; }
    public User Receiver { get; set; }

    [Required]
    public string Content { get; set; } // Mesaj içeriği

    public DateTime SentAt { get; set; } = DateTime.UtcNow; // Gönderilme zamanı

    public bool IsRead { get; set; } = false; // Mesajın okunup okunmadığı
}

}
