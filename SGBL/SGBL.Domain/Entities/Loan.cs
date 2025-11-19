using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("prestamos")]
    public class Loan
    {
        [Column("idprestamo")]
        [Key]
        public  int Id { get; set; }

        [Column("id_libro")]
        public int IdBook { get; set; }

        [Column("id_usuario")]
        public int IdUser { get; set; }

        [Column("id_bibliotecario")]
        public int? IdLibrarian { get; set; }

        [Column("fecha_prestamo")]
        public DateTime? DateLoan { get; set; }

        [Column("fecha_vencimiento")]
        public DateTime DueDate { get; set; }

        [Column("fecha_devolucion")]
        public DateTime? ReturnDate { get; set; }

        [Column("fecha_retiro")]
        public DateTime? PickupDate { get; set; }

        [Column("fecha_limite_retiro")]
        public DateTime PickupDeadline { get; set; }

        [Column("estado")]
        public int? Status { get; set; }

        [Column("monto_multa")]
        public decimal FineAmount { get; set; }

        [Column("notas")]
        public string? Notes { get; set; }

        // ⚠️ ELIMINA esta propiedad y usa solo CreationDate
        //[Column("fecha_creacion")]
        //public DateTime? CreatedAt { get; set; }

        [Column("fecha_creacion")]
        public DateTime? CreationDate { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime? UpdateDate { get; set; }

        // Relaciones de navegación
        [ForeignKey("IdBook")]
        public virtual Book Book { get; set; } = null!;

        [ForeignKey("IdUser")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("IdLibrarian")]
        public virtual User? Librarian { get; set; }

        [ForeignKey("Status")]
        public virtual LoanStatus? LoanStatus { get; set; }
    }
}