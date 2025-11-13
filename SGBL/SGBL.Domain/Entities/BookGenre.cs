

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("libros_generos")]
    public class BookGenre
    {
        [Column("id librosgeneros")]
        [Key]
        public  int Id { get; set; }
        [Column("id_libro")]
        public int IdBook { get; set; }
        [Column("id_genero")]
        public int IdGenre { get; set; }


        [ForeignKey("IdBook")]
        public virtual Book Book { get; set; }

        [ForeignKey("IdGenre")]
        public virtual Genre Genre { get; set; }
    }
}
