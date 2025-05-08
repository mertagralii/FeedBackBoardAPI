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
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}