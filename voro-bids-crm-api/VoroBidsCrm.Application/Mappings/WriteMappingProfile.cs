using AutoMapper;
using VoroBidsCrm.Application.DTOs;
using VoroBidsCrm.Domain.Entities;

namespace VoroBidsCrm.Application.Mappings
{
    public class WriteMappingProfile : Profile
    {
        public WriteMappingProfile()
        {
            CreateMap<UserExtensionDto, UserExtension>()
                .ForMember(d => d.UserId, o => o.Ignore());
        }
    }
}