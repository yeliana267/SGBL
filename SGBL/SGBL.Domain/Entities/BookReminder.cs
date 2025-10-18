

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("recordatorios_libros")]
    public class BookReminder : BaseEntity<int>
    {
        [Column("idrecordatorio")]
        [Key]
        public override int Id { get ; set ; }
        [Column("id_libro")]
        public int IdBook {  get; set ; }
        [Column("id_usuario")]
        public int IdUser { get; set ; }
        [Column("estado")]
        public int Status { get; set ; }
    }
}
