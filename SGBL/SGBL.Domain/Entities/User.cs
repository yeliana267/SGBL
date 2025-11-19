using SGBL.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGBL.Domain.Entities
{
    [Table("usuarios")]
    public class User : BaseEntity<int>
    {
        [Column("idusuario")]
        [Key]
        public override int Id { get; set; }

        [Column("nombre")]
        public string Name { get; set; } = string.Empty;

        [Column("email")]
        public string? Email { get; set; }

        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Column("rol")]
        public int Role { get; set; }

        [Column("estado")]
        public int Status { get; set; }

        [Column("token_activacion")]
        public string? TokenActivation { get; set; }

        [Column("token_recuperacion")]
        public string? TokenRecuperation { get; set; }

        [Column("token_activacion_expiracion")]
        public DateTime? TokenActivationExpires { get; set; }

        // Relaciones de navegación (agregar estas)
        [ForeignKey("Role")]
        public virtual Role UserRole { get; set; } = null!;

        [ForeignKey("Status")]
        public virtual UserStatus UserStatus { get; set; } = null!;
    }
}