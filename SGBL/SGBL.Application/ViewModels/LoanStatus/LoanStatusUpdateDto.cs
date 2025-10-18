using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.LoanStatus;

public sealed class LoanStatusUpdateDto
{
	[Required] public int Id { get; set; }
	[Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
	public bool IsActive { get; set; } = true;
}
