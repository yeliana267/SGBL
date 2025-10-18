using Application.Interfaces.Services;
using Application.ViewModels.LoanStatus;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/loan-statuses")]
public sealed class LoanStatusesController : ControllerBase
{
    private readonly ILoanStatusService _service;

    public LoanStatusesController(ILoanStatusService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? onlyActive, CancellationToken ct)
    {
        var list = await _service.ListAsync(onlyActive, ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LoanStatusCreateDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id }, null);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] LoanStatusUpdateDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Route id and body id differ.");

        var ok = await _service.UpdateAsync(dto, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
