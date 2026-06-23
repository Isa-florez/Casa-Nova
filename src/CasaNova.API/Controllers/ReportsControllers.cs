using CasaNova.Application.UseCases.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaNova.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReportsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDashboardQuery(), ct);
        return Ok(result);
    }

    [HttpGet("bookings/export")]
    public async Task<IActionResult> ExportExcel(CancellationToken ct)
    {
        var bytes = await _mediator.Send(new ExportBookingsExcelQuery(null, null), ct);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reservas.xlsx");
    }
}