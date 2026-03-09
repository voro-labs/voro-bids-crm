using VoroBidsCrm.Application.DTOs.CompanyDocuments;
using VoroBidsCrm.Application.Services.Interfaces.App.CompanyDocuments;
using VoroBidsCrm.Application.Services.Interfaces.Blob;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Shared.ViewModels;

namespace VoroBidsCrm.Application.Services.App.CompanyDocuments
{
    public class CompanyDocumentService : ICompanyDocumentService
    {
        private readonly ICompanyDocumentRepository _repository;
        private readonly IBlobService _blobService;

        public CompanyDocumentService(
            ICompanyDocumentRepository repository,
            IBlobService blobService)
        {
            _repository = repository;
            _blobService = blobService;
        }

        public async Task<ResponseViewModel<List<CompanyDocumentDto>>> GetAllAsync(Guid tenantId, CancellationToken ct = default)
        {
            var documents = await _repository.GetAllAsync(tenantId, ct);

            var dtos = documents.Select(d => new CompanyDocumentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                FileUrl = d.FileUrl,
                ExpirationDate = d.ExpirationDate,
                CreatedAt = d.CreatedAt
            }).ToList();

            return ResponseViewModel<List<CompanyDocumentDto>>.Success(dtos);
        }

        public async Task<ResponseViewModel<CompanyDocumentDto>> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default)
        {
            var document = await _repository.GetByIdAsync(id, tenantId, ct);

            if (document == null)
            {
                return ResponseViewModel<CompanyDocumentDto>.Fail("Documento não encontrado");
            }

            var dto = new CompanyDocumentDto
            {
                Id = document.Id,
                Name = document.Name,
                Description = document.Description,
                FileUrl = document.FileUrl,
                ExpirationDate = document.ExpirationDate,
                CreatedAt = document.CreatedAt
            };

            return ResponseViewModel<CompanyDocumentDto>.Success(dto);
        }

        public async Task<ResponseViewModel<CompanyDocumentDto>> CreateAsync(Guid tenantId, CreateCompanyDocumentDto dto, CancellationToken ct = default)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                return ResponseViewModel<CompanyDocumentDto>.Fail("Arquivo inválido", null, 400);
            }

            // Upload the file to Vercel Blob
            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            using var stream = dto.File.OpenReadStream();

            string fileUrl;
            try
            {
                fileUrl = await _blobService.UploadAsync(
                    blobName: $"company-documents/{tenantId}/{fileName}",
                    stream: stream,
                    contentType: dto.File.ContentType,
                    ct: ct);
            }
            catch (Exception ex)
            {
                return ResponseViewModel<CompanyDocumentDto>.Fail($"Erro no upload: {ex.Message}");
            }

            var document = new CompanyDocument
            {
                TenantId = tenantId,
                Name = dto.Name,
                Description = dto.Description,
                FileUrl = fileUrl,
                ExpirationDate = dto.ExpirationDate,
            };

            var created = await _repository.CreateAsync(document, ct);

            var resultDto = new CompanyDocumentDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                FileUrl = created.FileUrl,
                ExpirationDate = created.ExpirationDate,
                CreatedAt = created.CreatedAt
            };

            return ResponseViewModel<CompanyDocumentDto>.Success(resultDto);
        }

        public async Task<ResponseViewModel<object?>> DeleteAsync(Guid id, Guid tenantId, CancellationToken ct = default)
        {
            var document = await _repository.GetByIdAsync(id, tenantId, ct);
            if (document == null)
            {
                return ResponseViewModel<object?>.Fail("Documento não encontrado");
            }

            var deleted = await _repository.DeleteAsync(document, ct);

            if (!deleted)
            {
                return ResponseViewModel<object?>.Fail("Falha ao deletar o documento");
            }

            return ResponseViewModel<object?>.SuccessWithMessage("Documento deletado", null);
        }
    }
}
