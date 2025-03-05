using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    /// <summary>
    /// Контекст базы данных для работы с данными пользователей и долгов.
    /// </summary>
    public class LoanTrackerContext : DbContext
    {
        /// <summary>
        /// Коллекция пользователей в базе данных.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Коллекция долгов (IOU) в базе данных.
        /// </summary>
        public DbSet<Iou> Ious { get; set; }

        /// <summary>
        /// Конструктор контекста базы данных.
        /// </summary>
        public LoanTrackerContext(DbContextOptions<LoanTrackerContext> options) : base(options) { }

        /// <summary>
        /// Настройка модели для отношений между пользователями и долгами.
        /// </summary>
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
