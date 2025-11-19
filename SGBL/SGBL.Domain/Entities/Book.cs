
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("libros")]
    public class Book : BaseEntity<int>
    {
        [Column("idlibro")]
        [Key]
        public override int Id { get; set; }

        [Column("titulo")]
        public string Title { get; set; } = string.Empty;

        [Column("isbn")]
        public long Isbn { get; set; }

        [Column("descripcion")]
        public string? Description { get; set; } // Probablemente nullable

        [Column("año_publicacion")]
        public int PublicationYear { get; set; }

        [Column("paginas")]
        public int Pages { get; set; }

        [Column("copias_total")]
        public int TotalCopies { get; set; }

        [Column("copias_disponibles")]
        public int AvailableCopies { get; set; }

        [Column("ubicacion")]
        public string Ubication { get; set; } = string.Empty;

        [Column("estado")]
        public int Status { get; set; }

        // Relación con estado
        [ForeignKey("Status")]
        public virtual BookStatus BookStatus { get; set; } = null!;

        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>(); // Cambié el nombre por consistencia
    }

}
