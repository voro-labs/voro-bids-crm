using AutoMapper;
using Microsoft.Extensions.Logging;
using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.App.Auctions
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuctionService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public AuctionService(
            IAuctionRepository auctionRepository,
            IMapper mapper,
            ILogger<AuctionService> logger,
            ICurrentUserService currentUserService)
        {
            _auctionRepository = auctionRepository;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseViewModel<IEnumerable<AuctionDto>>> GetAllAsync()
        {
            try
            {
                var auctions = await _auctionRepository.GetAllAsync(a => !a.IsDeleted);
                var dtos = _mapper.Map<IEnumerable<AuctionDto>>(auctions);
                return ResponseViewModel<IEnumerable<AuctionDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all auctions");
                return ResponseViewModel<IEnumerable<AuctionDto>>.Fail("Ocorreu um erro ao buscar as licitações.");
            }
        }

        public async Task<ResponseViewModel<AuctionDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var auction = await _auctionRepository.GetByIdAsync(a => a.Id == id && !a.IsDeleted);

                if (auction == null)
                    return ResponseViewModel<AuctionDto>.Fail("Licitação não encontrada.");

                var dto = _mapper.Map<AuctionDto>(auction);
                return ResponseViewModel<AuctionDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting auction by id {Id}", id);
                return ResponseViewModel<AuctionDto>.Fail("Ocorreu um erro ao buscar a licitação.");
            }
        }

        public async Task<ResponseViewModel<AuctionDto>> CreateAsync(CreateAuctionDto createDto)
        {
            try
            {
                var auction = _mapper.Map<Auction>(createDto);
                auction.TenantId = _currentUserService.TenantId;

                await _auctionRepository.AddAsync(auction);

                await _auctionRepository.SaveChangesAsync();

                var dto = _mapper.Map<AuctionDto>(auction);
                return ResponseViewModel<AuctionDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating auction");
                return ResponseViewModel<AuctionDto>.Fail("Ocorreu um erro ao criar a licitação.");
            }
        }

        public async Task<ResponseViewModel<AuctionDto>> UpdateAsync(Guid id, UpdateAuctionDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return ResponseViewModel<AuctionDto>.Fail("O ID da licitação não corresponde.");

                var existingAuction = await _auctionRepository.GetByIdAsync(a => a.Id == id && !a.IsDeleted);

                if (existingAuction == null)
                    return ResponseViewModel<AuctionDto>.Fail("Licitação não encontrada.");

                _mapper.Map(updateDto, existingAuction);

                _auctionRepository.Update(existingAuction);
                await _auctionRepository.SaveChangesAsync();

                var dto = _mapper.Map<AuctionDto>(existingAuction);
                return ResponseViewModel<AuctionDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating auction {Id}", id);
                return ResponseViewModel<AuctionDto>.Fail("Ocorreu um erro ao atualizar a licitação.");
            }
        }

        public async Task<ResponseViewModel<object?>> DeleteAsync(Guid id)
        {
            try
            {
                var auction = await _auctionRepository.GetByIdAsync(a => a.Id == id && !a.IsDeleted);

                if (auction == null)
                    return ResponseViewModel<object?>.Fail("Licitação não encontrada.");

                // Soft delete is handled by interceptor in DbContext usually, or set it explicitly
                auction.IsDeleted = true;
                auction.DeletedAt = DateTimeOffset.UtcNow;

                _auctionRepository.Update(auction);
                await _auctionRepository.SaveChangesAsync();

                return ResponseViewModel<object?>.Success(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting auction {Id}", id);
                return ResponseViewModel<object?>.Fail("Ocorreu um erro ao deletar a licitação.");
            }
        }
    }
}
