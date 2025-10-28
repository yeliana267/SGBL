using Application.Interfaces.Services;
using Application.ViewModels.LoanStatus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class LoanStatusService : ILoanStatusService
{
    private readonly AppDbContext _db;

    public LoanStatusService(AppDbContext db) => _db = db;

    public async Task<int> CreateAsync(LoanStatusCreateDto dto, CancellationToken ct = default)
    {
        var entity = new LoanStatus { Name = dto.Name.Trim(), IsActive = dto.IsActive };
        _db.LoanStatuses.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(LoanStatusUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _db.LoanStatuses.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
        if (entity is null) return false;

        entity.Name = dto.Name.Trim();
        entity.IsActive = dto.IsActive;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.LoanStatuses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        _db.LoanStatuses.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<LoanStatusListItemDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.LoanStatuses
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new LoanStatusListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<LoanStatusListItemDto>> ListAsync(bool? onlyActive = null, CancellationToken ct = default)
    {
        var q = _db.LoanStatuses.AsNoTracking().AsQueryable();
        if (onlyActive is true) q = q.Where(x => x.IsActive);

        return await q
            .OrderBy(x => x.Name)
            .Select(x => new LoanStatusListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}
