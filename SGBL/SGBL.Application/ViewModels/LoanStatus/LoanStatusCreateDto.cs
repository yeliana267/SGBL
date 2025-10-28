using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.LoanStatus;

public sealed class LoanStatusCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
