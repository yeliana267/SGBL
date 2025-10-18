
using System.ComponentModel.DataAnnotations;


namespace SGBL.Application.Dtos.Nationality
{
    public class NationalityDto : BaseDto
    {
        [Required, StringLength(100)]

        public string Name { get; set; } = default!;
    }
}
