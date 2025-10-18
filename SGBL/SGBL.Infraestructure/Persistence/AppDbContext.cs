using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Configurations;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Book> Books => Set<Book>();
	public DbSet<Genre> Genres => Set<Genre>();
	public DbSet<BookGenre> BookGenres => Set<BookGenre>();
	public DbSet<LoanStatus> LoanStatuses => Set<LoanStatus>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new BookGenreConfiguration());
		modelBuilder.ApplyConfiguration(new LoanStatusConfiguration());
	}
}
