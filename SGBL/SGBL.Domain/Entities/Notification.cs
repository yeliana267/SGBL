using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("notificaciones")]
    public class Notification : BaseEntity<int>
    {
        [Key]
        [Column("idnotificacion")]
        public override int Id { get; set; }

        [Column("id_usuario")]
        public int IdUser { get; set; }

        [Column("tipo")]
        public int Type { get; set; }

        [Column("titulo")]
        public string Title { get; set; }

        [Column("mensaje")]
        public string Message { get; set; }

        [Column("estado")]
        public int? Status { get; set; }

        [Column("id_libro")]
        public int? IdBook { get; set; }

        [Column("id_prestamo")]
        public int? IdLoan { get; set; }

        [Column("fecha_lectura")]
        public DateTime? ReadDate { get; set; }
    }
}
