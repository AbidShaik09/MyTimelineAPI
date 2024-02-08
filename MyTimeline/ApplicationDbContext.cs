using Microsoft.EntityFrameworkCore;
using MyTimeline.Models;

namespace MyTimeline
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext>options):base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne()
                .HasForeignKey(t => t.UserId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.TimeSlots)
                .WithOne()
                .HasForeignKey(ts => ts.UserId)
                .IsRequired();
        }
        public DbSet<User> Users { get; set; }
    }

}
