using VoroBidsCrm.Application.Mappings;
using VoroBidsCrm.Application.Mappings.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace VoroBidsCrm.Contract.Extensions.Configurations
{
    public static class AddMappingExtension
    {
        public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<IdentityMappingProfile>();
                cfg.AddProfile<WriteMappingProfile>();
                cfg.AddProfile<ReadMappingProfile>();
            });

            return services;
        }
    }
}
