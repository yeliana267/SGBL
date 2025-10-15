

using System.ComponentModel.DataAnnotations.Schema;

namespace SGBL.Domain.Base
{
    public abstract class AuditEntity
    {
        [Column("fecha_creacion")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("fecha_actualizacion")]
        public DateTime? UpdatedAt { get; set; }
    }
}
