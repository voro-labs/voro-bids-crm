using VoroBidsCrm.Application.DTOs.Public;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface IPublicBookingService
    {
        Task<PublicTenantDto?> GetTenantBySlugAsync(string slug);
        Task<PublicClientDto?> CheckClientByPhoneAsync(string tenantSlug, string phone);
        Task<IEnumerable<PublicServiceDto>> GetServicesByTenantAsync(string tenantSlug);
        Task<IEnumerable<PublicEmployeeDto>> GetEmployeesByServiceAsync(string tenantSlug, Guid serviceId);
        Task<PublicBookingResponseDto> CreateBookingAsync(PublicBookingCreateDto dto);
        Task<IEnumerable<VoroBidsCrm.Application.DTOs.CRM.AvailabilitySlotDto>> GetAvailableSlotsAsync(string tenantSlug, DateTime date, Guid? serviceId = null, Guid? employeeId = null);
    }

    public record PublicBookingResponseDto(bool Success, string Message, Guid? AppointmentId);
}
