using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class BookViewModel : BaseViewModel<int>
    {

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ISBN es requerido")]
        [Range(1000000000000, 9999999999999, ErrorMessage = "ISBN debe tener 13 dígitos")]
        public long Isbn { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El año de publicación es requerido")]
        [Range(1000, 9999, ErrorMessage = "Año de publicación debe ser válido")]
        public int PublicationYear { get; set; }

        [Required(ErrorMessage = "El número de páginas es requerido")]
        [Range(1, 5000, ErrorMessage = "Páginas debe ser entre 1 y 5000")]
        public int Pages { get; set; }

        [Required(ErrorMessage = "El total de copias es requerido")]
        [Range(1, 1000, ErrorMessage = "Total de copias debe ser entre 1 y 1000")]
        public int TotalCopies { get; set; }

        [Required(ErrorMessage = "Las copias disponibles son requeridas")]
        [Range(0, 1000, ErrorMessage = "Copias disponibles debe ser entre 0 y 1000")]
        public int AvailableCopies { get; set; }

        [Required(ErrorMessage = "La ubicación es requerida")]
        [StringLength(100, ErrorMessage = "La ubicación no puede exceder 100 caracteres")]
        public string Ubication { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido")]
        [Display(Name = "Estado")]
        public int StatusId { get; set; }

        // Propiedad para mostrar los estados disponibles
        public List<BookStatusViewModel>? AvailableStatuses { get; set; }
        public List<int> SelectedAuthorIds { get; set; } = new List<int>();
        public List<AuthorViewModel> AvailableAuthors { get; set; } = new List<AuthorViewModel>();
        public List<AuthorViewModel> CurrentAuthors { get; set; } = new List<AuthorViewModel>();
    }
}