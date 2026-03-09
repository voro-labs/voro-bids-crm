using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoroBidsCrm.Application.Services;
using VoroBidsCrm.Application.Services.App.Auctions;
using VoroBidsCrm.Application.Services.Identity;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Application.Services.Interfaces.Blob;
using VoroBidsCrm.Application.Services.Interfaces.Email;
using VoroBidsCrm.Application.Services.Interfaces.Identity;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.Repositories.Identity;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Infrastructure.Blob;
using VoroBidsCrm.Infrastructure.Email;
using VoroBidsCrm.Infrastructure.Repositories;
using VoroBidsCrm.Infrastructure.Repositories.Identity;
using VoroBidsCrm.Infrastructure.Seeds;
using VoroBidsCrm.Infrastructure.UnitOfWork;
using VoroBidsCrm.Shared.Utils;

namespace VoroBidsCrm.Contract.Extensions.Configurations
{
    public static class AddAppServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("vercel-blob", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(100);
            });

            services.Configure<BlobUtil>(configuration.GetSection("BlobSettings"));
            services.Configure<MailUtil>(configuration.GetSection("EmailSettings"));
            services.Configure<CookieUtil>(configuration.GetSection("CookieSettings"));
            services.Configure<IntegrationUtil>(configuration.GetSection("IntegrationSettings"));

            services.AddScoped<IDataSeeder, DataSeeder>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IMailKitEmailService, MailKitEmailService>();

            #region Identity Repositories
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserExtensionRepository, UserExtensionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<ITenantModuleRepository, TenantModuleRepository>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IAuctionDocumentRepository, AuctionDocumentRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            #endregion

            #region Identity Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserExtensionService, UserExtensionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ITenantModuleService, TenantModuleService>();
            services.AddScoped<IAuctionService, AuctionService>();
            #endregion

            return services;
        }
    }
}
