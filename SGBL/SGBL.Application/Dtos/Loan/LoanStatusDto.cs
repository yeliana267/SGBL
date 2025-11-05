
namespace SGBL.Application.Dtos.Loan
{
    public class LoanStatusDto: BaseAuditableDto<int>
    {
        public string Name { get; set; } = default!;
    }
}
