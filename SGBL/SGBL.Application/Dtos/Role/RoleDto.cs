
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.Role
{
    public class RoleDto : BaseAuditableDto<int>
    {
        public string Name { get; set; } = default!;
    }
}

