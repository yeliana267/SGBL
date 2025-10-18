

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("notificacion_tipos")]
    public class NotificationType: BaseEntity<int>
    {
        [Column("idtipo")]
        [Key]
        public override int Id { get; set; }
        [Column("nombre")]
        public string Name { get; set; }
    }
}
