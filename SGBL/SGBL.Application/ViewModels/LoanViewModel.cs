using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class LoanViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El libro es requerido")]
        [Display(Name = "Libro")]
        public int IdBook { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        [Display(Name = "Usuario")]
        public int IdUser { get; set; }

        [Display(Name = "Bibliotecario")]
        public int? IdLibrarian { get; set; }

        [Display(Name = "Fecha de préstamo")]
        public DateTime? DateLoan { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
        [Display(Name = "Fecha de vencimiento")]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(15);

        [Display(Name = "Fecha de devolución")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Fecha de retiro")]
        public DateTime? PickupDate { get; set; }

        [Required(ErrorMessage = "La fecha límite de retiro es requerida")]
        [Display(Name = "Fecha límite de retiro")]
        public DateTime PickupDeadline { get; set; } = DateTime.Now.AddDays(2);

        [Display(Name = "Estado")]
        public int? Status { get; set; }

        [Display(Name = "Monto de multa")]
        public decimal FineAmount { get; set; }

        [Display(Name = "Notas")]
        public string? Notes { get; set; }
        public decimal CalculatedFine { get; set; }   // solo para mostrar en la UI

        // Propiedades de navegación para mostrar en vistas
        public string? BookTitle { get; set; }
        public string? UserName { get; set; }
        public string? LibrarianName { get; set; }
        public string? StatusName { get; set; }

  
    }
}