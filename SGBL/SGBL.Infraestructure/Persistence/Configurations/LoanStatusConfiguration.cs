using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class LoanStatusConfiguration : IEntityTypeConfiguration<LoanStatus>
{
    public void Configure(EntityTypeBuilder<LoanStatus> builder)
    {
        // Keep mapping to existing Spanish table (change to "loan_statuses" if you renamed the table)
        builder.ToTable("prestamo_estados");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .HasColumnName("idestado")
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .HasColumnName("nombre")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.IsActive).HasColumnName("activo");
        builder.Property(x => x.CreatedAt).HasColumnName("creado_en");

        builder.HasIndex(x => x.Name).IsUnique(false);
    }
}
