using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Application.DTOs.AuctionDocuments;
using VoroBidsCrm.Application.Services.Interfaces.App.Auctions;
using VoroBidsCrm.Application.Services.Interfaces.Blob;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.App.Auctions
{
    public class AuctionDocumentService : IAuctionDocumentService
    {
        private readonly IAuctionDocumentRepository _reqRepository;
        private readonly IDocumentFileRepository _fileRepository;
        private readonly IBlobService _blobService;
        private readonly IUnitOfWork _unitOfWork;

        public AuctionDocumentService(
            IAuctionDocumentRepository reqRepository,
            IDocumentFileRepository fileRepository,
            IBlobService blobService,
            IUnitOfWork unitOfWork)
        {
            _reqRepository = reqRepository;
            _fileRepository = fileRepository;
            _blobService = blobService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseViewModel<List<AuctionDocumentDto>>> GetAllByAuctionAsync(Guid auctionId, Guid tenantId, CancellationToken ct = default)
        {
            var requirements = await _reqRepository.Query()
                .Where(r => r.AuctionId == auctionId && r.TenantId == tenantId)
                .Select(r => new AuctionDocumentDto
                {
                    Id = r.Id,
                    AuctionId = r.AuctionId,
                    Name = r.Name,
                    Description = r.Description,
                    Status = (int)r.Status,
                    RequiresUpload = r.RequiresUpload,
                    ResponsibleId = r.ResponsibleId,
                    Files = r.Files.Where(f => !f.IsDeleted).Select(f => new DocumentFileDto
                    {
                        Id = f.Id,
                        AuctionDocumentId = f.AuctionDocumentId,
                        FileUrl = f.FileUrl,
                        FileName = f.FileName,
                        Version = f.Version,
                        UploadedById = f.UploadedById,
                        CreatedAt = f.CreatedAt
                    }).OrderByDescending(f => f.Version).ToList()
                })
                .ToListAsync(ct);

            return ResponseViewModel<List<AuctionDocumentDto>>.Success(requirements);
        }

        public async Task<ResponseViewModel<AuctionDocumentDto>> CreateRequirementAsync(Guid tenantId, CreateAuctionDocumentDto dto, CancellationToken ct = default)
        {
            var req = new AuctionDocument
            {
                TenantId = tenantId,
                AuctionId = dto.AuctionId,
                Name = dto.Name,
                Description = dto.Description,
                RequiresUpload = dto.RequiresUpload,
                ResponsibleId = dto.ResponsibleId,
                Status = DocumentStatus.Pending
            };

            await _reqRepository.AddAsync(req);
            await _unitOfWork.CommitAsync(ct);

            var result = new AuctionDocumentDto
            {
                Id = req.Id,
                AuctionId = req.AuctionId,
                Name = req.Name,
                Description = req.Description,
                RequiresUpload = req.RequiresUpload,
                ResponsibleId = req.ResponsibleId,
                Status = (int)req.Status,
                Files = []
            };

            return ResponseViewModel<AuctionDocumentDto>.Success(result);
        }

        public async Task<ResponseViewModel<DocumentFileDto>> UploadFileAsync(Guid tenantId, Guid userId, UploadDocumentFileDto dto, CancellationToken ct = default)
        {
            var requirement = await _reqRepository.GetByIdAsync(dto.AuctionDocumentId);
            if (requirement == null || requirement.TenantId != tenantId)
            {
                return ResponseViewModel<DocumentFileDto>.Fail("Requisito de documento não encontrado.");
            }

            if (dto.File == null || dto.File.Length == 0)
            {
                return ResponseViewModel<DocumentFileDto>.Fail("Arquivo inválido", null, 400);
            }

            var previousFilesCount = await _fileRepository.Query()
                .Where(f => f.AuctionDocumentId == dto.AuctionDocumentId)
                .CountAsync(ct);

            var version = previousFilesCount + 1;
            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";

            using var stream = dto.File.OpenReadStream();

            string fileUrl;
            try
            {
                fileUrl = await _blobService.UploadAsync(fileName, stream, dto.File.ContentType, ct: ct);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<DocumentFileDto>.Fail($"Erro no upload: {ex.Message}");
            }

            var docFile = new DocumentFile
            {
                TenantId = tenantId,
                AuctionDocumentId = dto.AuctionDocumentId,
                FileUrl = fileUrl,
                FileName = dto.File.FileName,
                Version = version,
                UploadedById = userId
            };

            await _fileRepository.AddAsync(docFile);

            // Atualiza status do requisito
            requirement.Status = DocumentStatus.Approved;
            _reqRepository.Update(requirement);
            await _unitOfWork.CommitAsync(ct);

            var resultDto = new DocumentFileDto
            {
                Id = docFile.Id,
                AuctionDocumentId = docFile.AuctionDocumentId,
                FileUrl = docFile.FileUrl,
                FileName = docFile.FileName,
                Version = docFile.Version,
                UploadedById = docFile.UploadedById,
                CreatedAt = docFile.CreatedAt
            };

            return ResponseViewModel<DocumentFileDto>.Success(resultDto);
        }

        public async Task<ResponseViewModel<object?>> DeleteRequirementAsync(Guid id, Guid tenantId, CancellationToken ct = default)
        {
            var requirement = await _reqRepository.GetByIdAsync(id);
            if (requirement == null || requirement.TenantId != tenantId)
            {
                return ResponseViewModel<object?>.Fail("Requisito não encontrado");
            }

            _reqRepository.Delete(requirement);
            await _unitOfWork.CommitAsync(ct);
            return ResponseViewModel<object?>.SuccessWithMessage("Requisito deletado", null);
        }

        public async Task<ResponseViewModel<object?>> DeleteFileAsync(Guid fileId, Guid tenantId, CancellationToken ct = default)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null || file.TenantId != tenantId)
            {
                return ResponseViewModel<object?>.Fail("Arquivo não encontrado");
            }

            _fileRepository.Delete(file);
            await _unitOfWork.CommitAsync(ct); // Ensure file is deleted first before counting remaining files

            // Re-avalia o status baseando-se em arquivos remanescentes
            var remainingFilesCount = await _fileRepository.Query()
                .Where(f => f.AuctionDocumentId == file.AuctionDocumentId && !f.IsDeleted)
                .CountAsync(ct);

            if (remainingFilesCount == 0 || remainingFilesCount == 1 && file.IsDeleted) // sometimes EF handles transaction reads differently
            {
                var requirement = await _reqRepository.GetByIdAsync(file.AuctionDocumentId);
                if (requirement != null)
                {
                    requirement.Status = DocumentStatus.Pending;
                    _reqRepository.Update(requirement);
                    await _unitOfWork.CommitAsync(ct);
                }
            }

            return ResponseViewModel<object?>.SuccessWithMessage("Arquivo deletado", null);
        }
    }
}
