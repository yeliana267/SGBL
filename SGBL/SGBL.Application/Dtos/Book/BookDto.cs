namespace SGBL.Application.Dtos.Book
{
    public class BookDto : BaseAuditableDto<int>
    {
        public string Title { get; set; } = string.Empty;
        public long Isbn { get; set; }
        public string? Description { get; set; }
        public int PublicationYear { get; set; }
        public int Pages { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string Ubication { get; set; } = string.Empty;
        public int StatusId { get; set; } 
    }
}