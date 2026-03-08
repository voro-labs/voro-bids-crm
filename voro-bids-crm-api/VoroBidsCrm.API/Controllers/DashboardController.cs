using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoroBidsCrm.Application.DTOs.CRM;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Shared.Extensions;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.API.Controllers
{
    [Route("api/v{version:version}/[controller]")]
    [Tags("Dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController(IDashboardService dashboardService, ICurrentUserService currentUserService) : ControllerBase
    {
        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            try
            {
                Console.WriteLine($"TenantId: {currentUserService.TenantId}");

                var metrics = await dashboardService.GetDashboardMetricsAsync();

                return ResponseViewModel<DashboardMetricsDto>
                    .SuccessWithMessage("Metrics retrieved.", metrics)
                    .ToActionResult();
            }
            catch (Exception ex)
            {
                return ResponseViewModel<object>.Fail(ex.Message).ToActionResult();
            }
        }
    }
}
