
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.Dtos.User
{
    public class UserStatusDto : BaseDto

    {
        [Required, StringLength(20)]

        public string Name { get; set; } = default!;
    }
}
