using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Interfaces.Entities;

namespace VoroBidsCrm.Domain.Entities
{
    public class ActivityLog : ITenantEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public Guid UserId { get; set; }
        public UserExtension User { get; set; } = null!;

        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;

        public Guid EntityId { get; set; }

        public string? Details { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
