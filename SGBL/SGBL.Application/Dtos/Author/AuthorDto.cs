
namespace SGBL.Application.Dtos.Author
{
    public class AuthorDto : BaseAuditableDto<int>
    {
       
        public string Name { get; set; }
        public string Biography { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Nationality { get; set; }
    }
}
