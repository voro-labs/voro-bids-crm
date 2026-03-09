using AutoMapper;
using VoroBidsCrm.Application.DTOs;
using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Domain.Entities;

namespace VoroBidsCrm.Application.Mappings
{
    public class WriteMappingProfile : Profile
    {
        public WriteMappingProfile()
        {
            CreateMap<UserExtensionDto, UserExtension>()
                .ForMember(d => d.UserId, o => o.Ignore());

            CreateMap<CreateAuctionDto, Auction>();
            CreateMap<UpdateAuctionDto, Auction>()
                .ForMember(d => d.Id, o => o.Ignore());
        }
    }
}