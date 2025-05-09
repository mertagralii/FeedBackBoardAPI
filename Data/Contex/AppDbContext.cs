using FeedBackBoardAPI.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FeedBackBoardAPI.Data.Contex;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Status> Statuses { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ApplicationUser - Feedback (1 - çok)
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.ApplicationUser)
            .WithMany(u => u.FeedBack)
            .HasForeignKey(f => f.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ApplicationUser - Comment (1 - çok)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ApplicationUser)
            .WithMany(u => u.Comment)
            .HasForeignKey(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Feedback - Comment (1 - çok)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Feedback)
            .WithMany(f => f.Comments)
            .HasForeignKey(c => c.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment - Replies (self-referencing)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}