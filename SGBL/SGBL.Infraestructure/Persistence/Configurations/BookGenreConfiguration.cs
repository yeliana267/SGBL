using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class BookGenreConfiguration : IEntityTypeConfiguration<BookGenre>
{
    public void Configure(EntityTypeBuilder<BookGenre> builder)
    {
        // Keep mapping to existing Spanish table (change to "book_genres" if you renamed the table)
        builder.ToTable("libro_generos");

        builder.HasKey(x => new { x.BookId, x.GenreId });

        builder.Property(x => x.BookId).HasColumnName("id_libro");
        builder.Property(x => x.GenreId).HasColumnName("id_genero");
        builder.Property(x => x.CreatedAt).HasColumnName("creado_en");

        builder.HasOne(x => x.Book)
               .WithMany(b => b.BookGenres)
               .HasForeignKey(x => x.BookId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Genre)
               .WithMany(g => g.BookGenres)
               .HasForeignKey(x => x.GenreId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
