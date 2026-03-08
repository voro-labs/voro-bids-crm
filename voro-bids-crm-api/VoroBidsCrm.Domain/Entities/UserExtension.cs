using System.ComponentModel.DataAnnotations;
using VoroBidsCrm.Domain.Entities.Identity;

namespace VoroBidsCrm.Domain.Entities
{
    public class UserExtension
    {
        [Key]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
