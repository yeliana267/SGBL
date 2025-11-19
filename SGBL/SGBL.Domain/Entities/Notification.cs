using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("notificaciones")]
    public class Notification : BaseEntity<int>
    {
        [Column("idnotificacion")]
        [Key]
        public override int Id { get; set; }

        [Column("id_usuario")]
        public int IdUser { get; set; }

        [Column("tipo")]
        public int Type { get; set; }

        [Column("titulo")]
        public string Title { get; set; } = string.Empty;

        [Column("mensaje")]
        public string Message { get; set; } = string.Empty;

        [Column("estado")]
        public int? Status { get; set; }

        [Column("id_libro")]
        public int? IdBook { get; set; }

        [Column("id_prestamo")]
        public int? IdLoan { get; set; }

        [Column("fecha_lectura")]
        public DateTime? ReadDate { get; set; }

        // Relaciones de navegación
        [ForeignKey("IdUser")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("IdBook")]
        public virtual Book? Book { get; set; }

        [ForeignKey("IdLoan")]
        public virtual Loan? Loan { get; set; }

        [ForeignKey("Status")]
        public virtual NotificationStatus? NotificationStatus { get; set; }

        [ForeignKey("Type")]
        public virtual NotificationType NotificationType { get; set; } = null!;
    }
}