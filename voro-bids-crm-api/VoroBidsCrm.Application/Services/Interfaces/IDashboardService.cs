using VoroBidsCrm.Application.DTOs.CRM;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardMetricsDto> GetDashboardMetricsAsync();
    }
}
