using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;

namespace SocialNetwork.Data
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("YourConnectionStringHere");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Определение отношений и ограничений
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserProfile>()
                .HasKey(up => up.UserId);

            modelBuilder.Entity<Post>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Chat>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.UserId, f.FriendId });

            modelBuilder.Entity<Friendship>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
