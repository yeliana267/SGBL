

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("prestamos")]
    public class Loan : BaseEntity<int>
    {
        [Key]
        [Column("idprestamo")]
        public override int Id { get; set; }

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
        public int Status { get; set; }

        [Column("monto_multa")]
        public decimal FineAmount { get; set; }

        [Column("notas")]
        public string? Notes { get; set; }
    }
}

