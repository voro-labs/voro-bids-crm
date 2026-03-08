using VoroBidsCrm.Application.DTOs.Tenant;

namespace VoroBidsCrm.Application.DTOs.Auth
{
    public record SessionUserDto(
        Guid Id,
        string Name,
        string Email,
        string Role
    );

    public record SessionDto(
        SessionUserDto User,
        TenantDto Tenant
    );
}
