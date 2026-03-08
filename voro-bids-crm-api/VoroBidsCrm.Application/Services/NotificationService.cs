using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoroBidsCrm.Application.Services.Base;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.Email;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Shared.Extensions;

namespace VoroBidsCrm.Application.Services
{
    public class NotificationService(IMailKitEmailService emailService, IConfiguration configuration, INotificationRepository notificationRepository) : ServiceBase<Notification>(notificationRepository), INotificationService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IMailKitEmailService _emailService = emailService;
        private readonly INotificationRepository _notificationRepository = notificationRepository;

        private string UrlBase =>
            _configuration
                .GetSection("CorsSettings")
                .GetSection("AllowedOrigins")
                .Get<string[]>()?[0] ?? "{UrlBase}";

        public async Task SendWelcomeAsync(string email, string userName)
        {
            var template = await _notificationRepository
                .Query(n => n.Name == NotificationEnum.Welcome.AsText() && n.IsActive).FirstOrDefaultAsync();

            if (template == null)
                throw new InvalidOperationException("Template de e-mail de recepção não encontrado.");

            // Substitui placeholders no corpo e no assunto
            var subject = template.Subject
                .Replace("{UserName}", userName);

            var body = template.Body
                .Replace("{UserName}", userName);

            // Envia o e-mail usando o serviço de e-mail real
            await _emailService.SendAsync(email, subject, body, template.Cc, template.Bcc);
        }

        public async Task SendResetLinkAsync(string email, string userName, string token)
        {
            var template = await _notificationRepository
                .Query(n => n.Name == NotificationEnum.PasswordReset.AsText() && n.IsActive).FirstOrDefaultAsync();

            if (template == null)
                throw new InvalidOperationException("Template de e-mail de reset de senha não encontrado.");

            // Gera o link de reset com codificação de URL (evita perder + ou =)
            var resetLink = $"{UrlBase}/admin/reset-password?email={Uri.EscapeDataString(email)}&token={token}";

            // Substitui placeholders no corpo e no assunto
            var subject = template.Subject
                .Replace("{UserName}", userName);

            var body = template.Body
                .Replace("{UserName}", userName)
                .Replace("{ResetLink}", resetLink);

            // Envia o e-mail usando o serviço de e-mail real
            await _emailService.SendAsync(email, subject, body, template.Cc, template.Bcc);
        }

        public async Task SendConfirmEmailAsync(string email, string userName, string token)
        {
            var template = await _notificationRepository
                .Query(n => n.Name == NotificationEnum.ConfirmEmail.AsText() && n.IsActive).FirstOrDefaultAsync();

            if (template == null)
                throw new InvalidOperationException("Template de e-mail de confirmação de e-mail não encontrado.");

            // Gera o link de confirmação com codificação de URL
            var confirmLink = $"{UrlBase}/admin/confirm-email?email={Uri.EscapeDataString(email)}&token={token}";

            // Substitui placeholders no corpo e no assunto
            var subject = template.Subject
                .Replace("{UserName}", userName);

            var body = template.Body
                .Replace("{UserName}", userName)
                .Replace("{ConfirmLink}", confirmLink);

            // Envia o e-mail usando o serviço de e-mail real
            await _emailService.SendAsync(email, subject, body, template.Cc, template.Bcc);
        }
    }
}
