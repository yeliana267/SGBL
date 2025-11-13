

using Microsoft.EntityFrameworkCore;
using SGBL.Domain.Entities;

namespace SGBL.Persistence.Context
{
    public class SGBLContext: DbContext
    {
        public SGBLContext(DbContextOptions<SGBLContext> options) : base(options)
        {
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BookRating> BookRatings { get; set; }
        public DbSet<BookReminder> BookReminders { get; set; }
        public DbSet<BookStatus> BookStatuses { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanStatus> LoanStatuses { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationStatus> NotificationStatuses { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<ReminderStatus> ReminderStatuses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.IdBook);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.IdAuthor);

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("prestamos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("idprestamo");
                entity.Property(e => e.IdBook).HasColumnName("id_libro");
                entity.Property(e => e.IdUser).HasColumnName("id_usuario");
                entity.Property(e => e.IdLibrarian).HasColumnName("id_bibliotecario");
                entity.Property(e => e.DateLoan).HasColumnName("fecha_prestamo");
                entity.Property(e => e.DueDate).HasColumnName("fecha_vencimiento");
                entity.Property(e => e.ReturnDate).HasColumnName("fecha_devolucion");
                entity.Property(e => e.PickupDate).HasColumnName("fecha_retiro");
                entity.Property(e => e.PickupDeadline).HasColumnName("fecha_limite_retiro");
                entity.Property(e => e.Status).HasColumnName("estado");
                entity.Property(e => e.FineAmount).HasColumnName("monto_multa");
                entity.Property(e => e.Notes).HasColumnName("notas");
                entity.Property(e => e.CreatedAt).HasColumnName("fecha_creacion");
                entity.Property(e => e.UpdatedAt).HasColumnName("fecha_actualizacion");
            });
        }
    }
}
