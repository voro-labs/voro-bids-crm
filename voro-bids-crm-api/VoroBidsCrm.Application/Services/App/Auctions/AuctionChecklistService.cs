using VoroBidsCrm.Application.DTOs.Auctions;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.App.Auctions
{
    public class AuctionChecklistService(
        IAuctionChecklistRepository checklistRepository,
        IAuctionRepository auctionRepository,
        IUnitOfWork unitOfWork) : IAuctionChecklistService
    {
        public async Task<ResponseViewModel<IEnumerable<AuctionChecklistDto>>> GetByAuctionIdAsync(Guid auctionId)
        {
            var auction = await auctionRepository.GetByIdAsync(auctionId);
            if (auction == null)
                return ResponseViewModel<IEnumerable<AuctionChecklistDto>>.Fail("Licitação não encontrada.");

            var checklists = await checklistRepository.GetAllAsync(x => x.AuctionId == auctionId);

            var dtos = checklists.OrderBy(c => c.CreatedAt).Select(c => new AuctionChecklistDto
            {
                Id = c.Id,
                AuctionId = c.AuctionId,
                TaskName = c.TaskName,
                IsCompleted = c.IsCompleted,
                CreatedAt = c.CreatedAt
            });

            return ResponseViewModel<IEnumerable<AuctionChecklistDto>>.Success(dtos);
        }

        public async Task<ResponseViewModel<AuctionChecklistDto>> CreateAsync(CreateAuctionChecklistDto createDto, Guid tenantId)
        {
            var auction = await auctionRepository.GetByIdAsync(createDto.AuctionId);
            if (auction == null)
                return ResponseViewModel<AuctionChecklistDto>.Fail("Licitação não encontrada.");

            var checklist = new AuctionChecklist
            {
                TenantId = tenantId,
                AuctionId = createDto.AuctionId,
                TaskName = createDto.TaskName,
                IsCompleted = false
            };

            await checklistRepository.AddAsync(checklist);
            await unitOfWork.CommitAsync();

            var dto = new AuctionChecklistDto
            {
                Id = checklist.Id,
                AuctionId = checklist.AuctionId,
                TaskName = checklist.TaskName,
                IsCompleted = checklist.IsCompleted,
                CreatedAt = checklist.CreatedAt
            };

            return ResponseViewModel<AuctionChecklistDto>.Success(dto);
        }

        public async Task<ResponseViewModel<AuctionChecklistDto>> UpdateAsync(Guid id, UpdateAuctionChecklistDto updateDto)
        {
            var checklist = await checklistRepository.GetByIdAsync(id);
            if (checklist == null)
                return ResponseViewModel<AuctionChecklistDto>.Fail("Tarefa não encontrada.");

            checklist.TaskName = updateDto.TaskName;
            checklist.IsCompleted = updateDto.IsCompleted;
            checklist.UpdatedAt = DateTimeOffset.UtcNow;

            checklistRepository.Update(checklist);
            await unitOfWork.CommitAsync();

            var dto = new AuctionChecklistDto
            {
                Id = checklist.Id,
                AuctionId = checklist.AuctionId,
                TaskName = checklist.TaskName,
                IsCompleted = checklist.IsCompleted,
                CreatedAt = checklist.CreatedAt
            };

            return ResponseViewModel<AuctionChecklistDto>.Success(dto);
        }

        public async Task<ResponseViewModel<object?>> ToggleCompletionAsync(Guid id)
        {
            var checklist = await checklistRepository.GetByIdAsync(id);
            if (checklist == null)
                return ResponseViewModel<object?>.Fail("Tarefa não encontrada.");

            checklist.IsCompleted = !checklist.IsCompleted;
            checklist.UpdatedAt = DateTimeOffset.UtcNow;

            checklistRepository.Update(checklist);
            await unitOfWork.CommitAsync();

            return ResponseViewModel<object?>.SuccessWithMessage("Status da tarefa atualizado com sucesso.", null);
        }

        public async Task<ResponseViewModel<object?>> DeleteAsync(Guid id)
        {
            var checklist = await checklistRepository.GetByIdAsync(id);
            if (checklist == null)
                return ResponseViewModel<object?>.Fail("Tarefa não encontrada.");

            checklistRepository.Delete(checklist);
            await unitOfWork.CommitAsync();

            return ResponseViewModel<object?>.SuccessWithMessage("Tarefa excluída com sucesso.", null);
        }
    }
}
