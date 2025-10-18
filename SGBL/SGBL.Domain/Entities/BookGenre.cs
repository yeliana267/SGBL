

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("libros_generos")]
    public class BookGenre: BaseEntity<int>
    {
        [Column("id librosgeneros")]
        [Key]
        public override int Id { get; set; }
        [Column("id_libro")]
        public int IdBook { get; set; }
        [Column("id_genero")]
        public int IdGenre { get; set; }
    }
}
