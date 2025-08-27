using Microsoft.EntityFrameworkCore;
using UniFinderWebProg.Models;

namespace UniFinderWebProg.Utulity
{
    public class UygulamaDbContext:DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options):base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ProfilePreferences> ProfilePreferences { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Match ile User1 ve User2 arasındaki ilişkileri manuel olarak yapılandırıyoruz
            modelBuilder.Entity<Match>()
                .HasOne(m => m.User1)
                .WithMany(u => u.MatchesAsUser1)
                .HasForeignKey(m => m.User1Id)
                .OnDelete(DeleteBehavior.Restrict);  // Eşleşme silindiğinde, kullanıcıyı silme

            modelBuilder.Entity<Match>()
                .HasOne(m => m.User2)
                .WithMany(u => u.MatchesAsUser2)
                .HasForeignKey(m => m.User2Id)
                .OnDelete(DeleteBehavior.Restrict);  // Eşleşme silindiğinde, kullanıcıyı silme
        }




    }
}
