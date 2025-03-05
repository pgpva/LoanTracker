using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    public class LoanTrackerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Iou> Ious { get; set; }

        public LoanTrackerContext(DbContextOptions<LoanTrackerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Iou>()
                .HasOne(i => i.Lender)
                .WithMany(u => u.Owes)
                .HasForeignKey(i => i.LenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Iou>()
                .HasOne(i => i.Borrower)
                .WithMany(u => u.OwedBy)
                .HasForeignKey(i => i.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
