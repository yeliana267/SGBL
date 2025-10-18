

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("libro_estados")]
    public class BookStatus: BaseEntity<int>
    {
        [Column("idestado")]
        [Key]
        public override int Id { get; set; }
        [Column("nombre")]
        public string Name { get; set; }
    }
}
