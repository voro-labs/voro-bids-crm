using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class TenantModule : ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public AppModule Module { get; set; }
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// JSON configuration for the module (e.g., custom display names)
        /// </summary>
        public string? ConfigurationJson { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
