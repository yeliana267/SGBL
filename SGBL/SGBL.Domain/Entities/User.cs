
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("usuarios")]
    public class User: BaseEntity<int>
    {
        [Column("idusuario")]
        [Key]
        public override int Id { get; set; }
        [Column("nombre")]
        public string Name { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("rol")]
        public int Role { get; set; }
        [Column("estado")]
        public int Status { get; set; }
        [Column("token_activacion")]
        public string TokenActivation { get; set; }
        [Column("token_recuperacion")]
        public string TokenRecuperation { get; set; }
        [Column("token_activacion_expiracion")]
        public DateTime? TokenActivationExpires { get; set; }
    }
}
