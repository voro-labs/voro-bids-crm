using VoroBidsCrm.Application.DTOs.Identity;
using VoroBidsCrm.Domain.Enums;

namespace VoroBidsCrm.Application.DTOs
{
    public class UserExtensionDto
    {
        public Guid? UserId { get; set; }
        public UserDto? User { get; set; }
    }
}
