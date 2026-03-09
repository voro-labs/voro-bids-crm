using AutoMapper;
using VoroBidsCrm.Application.DTOs;
using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Domain.Entities;

namespace VoroBidsCrm.Application.Mappings
{
    public class ReadMappingProfile : Profile
    {
        public ReadMappingProfile()
        {
            CreateMap<UserExtension, UserExtensionDto>();
            CreateMap<Auction, AuctionDto>();
        }
    }
}