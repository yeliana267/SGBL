

using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("usuario_estados")]
    public class UserStatus: BaseEntity<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
    }
}
