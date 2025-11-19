using SGBL.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGBL.Domain.Entities
{
    [Table("autores")]
    public class Author : BaseEntity<int>
    {
        [Column("idautor")]
        [Key]
        public override int Id { get; set; }

        [Column("nombre")]
        public string Name { get; set; } = string.Empty;

        [Column("biografia")]
        public string? Biography { get; set; } // Probablemente nullable

        [Column("fecha_nacimiento", TypeName = "date")]
        public DateTime? BirthDate { get; set; } // Probablemente nullable

        [Column("nacionalidad")]
        public int? Nationality { get; set; } // Probablemente nullable

        [ForeignKey("Nationality")]
        public virtual Nationality? AuthorNationality { get; set; }

        public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}