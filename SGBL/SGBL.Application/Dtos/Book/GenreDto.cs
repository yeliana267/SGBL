
namespace SGBL.Application.Dtos.Book
{
    public class GenreDto : BaseAuditableDto<int>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
