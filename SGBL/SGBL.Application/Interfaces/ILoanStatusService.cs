using Application.ViewModels.LoanStatus;

namespace Application.Interfaces.Services;

public interface ILoanStatusService
{
    Task<int> CreateAsync(LoanStatusCreateDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(LoanStatusUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<LoanStatusListItemDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<LoanStatusListItemDto>> ListAsync(bool? onlyActive = null, CancellationToken ct = default);
}
