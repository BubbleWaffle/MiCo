using MiCo.Models;
using Microsoft.EntityFrameworkCore;

namespace MiCo.Data
{
    public class MiCoDbContext : DbContext
    {
        public MiCoDbContext(DbContextOptions<MiCoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Threads>()
                .HasOne(t => t.reply)
                .WithMany()
                .HasForeignKey(t => t.id_reply);

            modelBuilder.Entity<Threads>()
                .HasOne(t => t.OG_thread)
                .WithMany()
                .HasForeignKey(t => t.id_OG_thread);

            modelBuilder.Entity<Threads>()
                .HasMany(t => t.thread_images)
                .WithOne(ti => ti.which_thread)
                .HasForeignKey(ti => ti.id_which_thread)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bans>()
                .HasOne(b => b.banned_user)
                .WithMany()
                .HasForeignKey(b => b.id_banned_user)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bans>()
                .HasOne(b => b.moderator)
                .WithMany()
                .HasForeignKey(b => b.id_moderator)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reports>()
                .HasOne(r => r.reporting_user)
                .WithMany()
                .HasForeignKey(r => r.id_reporting_user)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reports>()
                .HasOne(r => r.reported_user)
                .WithMany()
                .HasForeignKey(r => r.id_reported_user)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.user)
                .WithMany()
                .HasForeignKey(l => l.id_user)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.thread)
                .WithMany()
                .HasForeignKey(l => l.id_thread)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThreadTags>().HasKey(tt => new { tt.id_thread, tt.id_tag });

            /* Add administrator acc */
            modelBuilder.Entity<Users>().HasData(
                    new Users
                    {
                        id = 1,
                        nickname = "admin",
                        login = "admin",
                        password = Utilities.Tools.HashPassword("Admin#123"),
                        email = "admin@mail.mod",
                        creation_date = DateTimeOffset.UtcNow,
                        role = 1,
                        status = 0
                    });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Users> users { get; set; }
        public DbSet<Reports> reports { get; set; }
        public DbSet<Bans> bans { get; set; }
        public DbSet<Likes> likes { get; set; }
        public DbSet<Threads> threads { get; set; }
        public DbSet<ThreadTags> thread_tags { get; set; }
        public DbSet<Tags> tags { get; set; }
        public DbSet<Images> images { get; set; }
    }
}