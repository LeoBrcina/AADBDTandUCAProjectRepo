using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PicGramWebApp.Models;

namespace PicGramWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<PackagePlan> PackagePlans { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<PhotoHashtag> PhotoHashtags { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<PackageChangeRequest> PackageChangeRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PhotoHashtag>()
                .HasKey(ph => new { ph.PhotoId, ph.HashtagId });

            builder.Entity<PhotoHashtag>()
                .HasOne(ph => ph.Photo)
                .WithMany(p => p.PhotoHashtags)
                .HasForeignKey(ph => ph.PhotoId);

            builder.Entity<PhotoHashtag>()
                .HasOne(ph => ph.Hashtag)
                .WithMany(h => h.PhotoHashtags)
                .HasForeignKey(ph => ph.HashtagId);

            builder.Entity<PackageChangeRequest>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PackageChangeRequest>()
                .HasOne(r => r.CurrentPackagePlan)
                .WithMany()
                .HasForeignKey(r => r.CurrentPackagePlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PackageChangeRequest>()
                .HasOne(r => r.RequestedPackagePlan)
                .WithMany()
                .HasForeignKey(r => r.RequestedPackagePlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
