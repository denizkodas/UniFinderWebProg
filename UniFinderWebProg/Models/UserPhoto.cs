namespace UniFinderWebProg.Models
{
    public class UserPhoto
    {
        public int Id { get; set; }       
        public string? PhotoPath { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }  
    }
}
