using SGBL.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGBL.Domain.Entities
{
    [Table("recordatorios_libros")]
    public class BookReminder : BaseEntity<int>
    {
        [Column("idrecordatorio")]
        [Key]
        public override int Id { get; set; }

        [Column("id_libro")]
        public int IdBook { get; set; }

        [Column("id_usuario")]
        public int IdUser { get; set; }

        [Column("estado")]
        public int Status { get; set; }

        // Relaciones de navegación
        [ForeignKey("IdBook")]
        public virtual Book Book { get; set; } = null!;

        [ForeignKey("IdUser")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("Status")]
        public virtual ReminderStatus ReminderStatus { get; set; } = null!;
    }
}