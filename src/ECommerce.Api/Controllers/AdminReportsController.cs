using ECommerce.Application.Features.Reports.RevenueReport;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/admin/reports")]
[Authorize(Roles = Roles.Admin)]
public class AdminReportsController : ControllerBase
{
    private readonly ISender _mediator;

    public AdminReportsController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/admin/reports/revenue?from=&amp;to= — revenue totals, by-day series, top products (UC-34).</summary>
    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueReportDto>> Revenue(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var toDate = to ?? DateTime.UtcNow;
        var fromDate = from ?? toDate.AddDays(-30);
        return Ok(await _mediator.Send(new RevenueReportQuery(fromDate, toDate), cancellationToken));
    }
}
