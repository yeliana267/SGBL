

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("libros_autores")]
    public class BookAuthor : BaseEntity<int>
    {
        [Column("idlibroautor")]
        [Key]
        public override int Id { get; set; }
        [Column("idlibro")]
        public int IdBook { get; set; }
        [Column("idautor")]
        public int IdAuthor { get; set; }
    }
}
