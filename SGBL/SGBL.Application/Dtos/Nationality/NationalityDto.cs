
using System.ComponentModel.DataAnnotations;


namespace SGBL.Application.Dtos.Nationality
{
    public class NationalityDto : BaseAuditableDto<int>
    {
        public string Name { get; set; } = default!;
    }
}
