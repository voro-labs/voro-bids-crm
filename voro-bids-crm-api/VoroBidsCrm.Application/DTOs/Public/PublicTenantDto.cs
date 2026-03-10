namespace VoroBidsCrm.Application.DTOs.Public
{
    public record PublicTenantDto(
        Guid Id,
        string Name,
        string Slug,
        string? ContactPhone,
        string? LogoUrl,
        string? PrimaryColor,
        string? SecondaryColor,
        string? ThemeMode
    );
}