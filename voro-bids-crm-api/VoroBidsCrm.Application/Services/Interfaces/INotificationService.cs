using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Application.Services.Interfaces.Base;

namespace VoroBidsCrm.Application.Services.Interfaces
{
    public interface INotificationService : IServiceBase<Notification>
    {
        Task SendWelcomeAsync(string email, string userName);
        Task SendResetLinkAsync(string email, string userName, string token);
        Task SendConfirmEmailAsync(string email, string userName, string token);
    }
}
