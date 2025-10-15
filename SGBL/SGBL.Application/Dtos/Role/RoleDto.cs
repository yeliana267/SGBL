
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.Role
{
    public class RoleDto : BaseDto
    {
        [Required, StringLength(100)]
      
        public string Name { get; set; } = default!;
    }
}

