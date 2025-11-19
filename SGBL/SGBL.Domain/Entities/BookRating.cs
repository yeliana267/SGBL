using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("calificaciones_libros")]
    public class BookRating: BaseEntity<int>
    {
        [Column("idcalificacion")]
        [Key]
        public override int Id { get; set; }
        [Column("id_libro")]
        public int IdBook {  get; set; }
        [Column("id_usuario")]
        public int IdUser { get; set; }
        [Column("calificacion")]
        public int Rating { get; set; }
        [Column("comentario")]
        public string Comment { get; set; }
    }
}
