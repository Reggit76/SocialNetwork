using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.Entity;
using SocialNetwork.Services;

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
        public DbSet<ChatUser> ChatUsers { get; set; } // Промежуточная таблица

        public UserDbContext()
        {
            Database.EnsureCreated();
        }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SNDB;Username=postgres;Password=11111111");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            // UserProfile entity configuration
            modelBuilder.Entity<UserProfile>()
                .HasKey(up => up.UserId);

            modelBuilder.Entity<UserProfile>()
                .Property(up => up.FullName)
                .HasMaxLength(100);

            modelBuilder.Entity<UserProfile>()
                .Property(up => up.ProfilePictureUrl)
                .HasMaxLength(255);

            modelBuilder.Entity<UserProfile>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserProfile>(up => up.UserId);

            // Post entity configuration
            modelBuilder.Entity<Post>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Post>()
                .Property(p => p.Content)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .Property(p => p.DatePosted)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment entity configuration
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Comment>()
                .Property(c => c.Content)
                .IsRequired();

            modelBuilder.Entity<Comment>()
                .Property(c => c.Timestamp)
                .IsRequired();

            modelBuilder.Entity<Comment>()
                .HasOne<Post>()
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Chat entity configuration
            modelBuilder.Entity<Chat>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Chat>()
                .Property(c => c.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Chat>()
                .Property(c => c.Description)
                .HasMaxLength(255);

            // Message entity configuration
            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .Property(m => m.Content)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(m => m.Timestamp)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .HasOne<Chat>()
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Friendship entity configuration
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

            // ChatUser entity configuration (many-to-many relationship)
            modelBuilder.Entity<ChatUser>()
                .HasKey(cu => new { cu.ChatId, cu.UserId });

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.Chat)
                .WithMany(c => c.Participants)
                .HasForeignKey(cu => cu.ChatId);

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.UserProfile)
                .WithMany(up => up.Chats)
                .HasForeignKey(cu => cu.UserId);
        }
    }
}
