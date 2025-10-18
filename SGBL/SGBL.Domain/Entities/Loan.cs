

using System.ComponentModel.DataAnnotations.Schema;
using SGBL.Domain.Base;

namespace SGBL.Domain.Entities
{
    [Table("prestamos")]
    public class Loan: BaseEntity<int>
    {
        public override int Id { get; set; }
        public int IdBook { get; set; }
        public int IdUser { get; set; }
        public int IdLibrarian { get; set; }
        public DateTime DateLoan { get; set; }
        public DateTime DueDate {  get; set; }
        public DateTime ReturnDate {  get; set; }
        public DateTime PickupDate {  get; set; }
        public DateTime PickupDeadline { get; set; }
        public int Status { get; set; }
        public decimal FineAmount { get; set; }
        public string Notes { get; set; }
    }
}
