namespace SGBL.Application.Dtos.Loan
{
    public class LoanDto
    {
        public int Id { get; set; }
        public int IdBook { get; set; }
        public int IdUser { get; set; }
        public int? IdLibrarian { get; set; }

        public DateTime? DateLoan { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime PickupDeadline { get; set; }

        public int? Status { get; set; }
        public decimal FineAmount { get; set; }
        public string? Notes { get; set; }

        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public string? BookTitle { get; set; }
        public string? UserName { get; set; }
        public string? LibrarianName { get; set; }
        public string? StatusName { get; set; }
    }
}
