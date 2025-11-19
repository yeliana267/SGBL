namespace SGBL.Application.Dtos.Book
{
    public class BookStatusDto : BaseAuditableDto<int>
    {
        public string Name { get; set; } =default!;
    }
}
