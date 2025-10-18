

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("generos")]
    public class Genre: BaseEntity<int>
    {
        [Column("idgenero")]
        [Key]
        public override int Id { get; set; }
        [Column("nombre")]
        public string Name { get; set; }
        [Column("descripcion")]
        public string Description { get; set; }
    }
}
