namespace VoroBidsCrm.Application.DTOs.Tenants
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ContactPhone { get; set; }
        public string? DocumentDetails { get; set; }
        public string? StreetAddress { get; set; }
        public string? Number { get; set; }
        public string? Complement { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? LogoUrl { get; set; }
    }
}
