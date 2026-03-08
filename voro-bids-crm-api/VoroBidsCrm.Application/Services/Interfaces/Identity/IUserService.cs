using VoroBidsCrm.Application.DTOs;
using VoroBidsCrm.Application.DTOs.Identity;
using VoroBidsCrm.Application.Services.Interfaces.Base;
using VoroBidsCrm.Domain.Entities.Identity;

namespace VoroBidsCrm.Application.Services.Interfaces.Identity
{
    public interface IUserService : IServiceBase<User>
    {
        Task<(User user, IList<string>? rolesNames)> GetByEmailAndPassword(string email, string password);
        Task<User> CreateAsync(UserDto dto, string password, ICollection<string> roles);
        Task<User> UpdateAsync(Guid id, UserDto dto);
        Task<(User user, string token)> GenerateConfirmEmailAsync(string email);
        Task<bool> ConfirmEmailAsync(AuthDto authViewModel, string email);
        Task<(User user, string token)> GenerateForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<IList<string>> GetRolesAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
    }
}
