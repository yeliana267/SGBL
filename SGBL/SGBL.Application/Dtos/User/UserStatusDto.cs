
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.User
{
    public class UserStatusDto : BaseAuditableDto<int>

    {
        public string Name { get; set; } = default!;
    }
}
