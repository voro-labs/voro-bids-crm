using VoroBidsCrm.Application.DTOs.Public;
using VoroBidsCrm.Application.DTOs.Tenant;
using VoroBidsCrm.Application.Services.Base;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Application.Services.Interfaces.Blob;
using VoroBidsCrm.Domain.Entities;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;

namespace VoroBidsCrm.Application.Services
{
    public class TenantService(ITenantRepository tenantRepository, IUnitOfWork unitOfWork, IBlobService blobService) : ServiceBase<Tenant>(tenantRepository), ITenantService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBlobService _blobService = blobService;

        public async Task<Tenant> CreateAsync(CreateTenantDto dto)
        {
            var existing = await _tenantRepository.GetBySlugAsync(dto.Slug);
            if (existing is not null)
                throw new InvalidOperationException($"A tenant with slug '{dto.Slug}' already exists.");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Slug = dto.Slug.ToLowerInvariant().Trim(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _tenantRepository.AddAsync(tenant);
            await _unitOfWork.SaveChangesAsync();

            return tenant;
        }

        public async Task<Tenant?> GetByIdAsync(Guid id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant is null) return null;

            if (!string.IsNullOrEmpty(tenant.LogoUrl))
            {
                tenant.LogoUrl = await _blobService.GetSignedUrlAsync(tenant.LogoUrl);
            }

            return tenant;
        }

        public async Task<PublicTenantDto?> GetBySlugAsync(string slug)
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null) return null;

            string? logoUrl = null;

            if (!string.IsNullOrEmpty(tenant.LogoUrl))
            {
                logoUrl = await _blobService.GetSignedUrlAsync(tenant.LogoUrl);
            }

            return new PublicTenantDto(
                tenant.Id,
                tenant.Name,
                tenant.Slug,
                tenant.ContactPhone,
                logoUrl,
                tenant.PrimaryColor,
                tenant.SecondaryColor,
                tenant.ThemeMode?.ToString()
            );
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _tenantRepository.GetAllAsync();
        }

        public async Task<Tenant> UpdateAsync(Guid id, UpdateTenantDto dto)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Tenant '{id}' not found.");

            if (dto.Name is not null) tenant.Name = dto.Name;
            if (dto.Slug is not null) tenant.Slug = dto.Slug.ToLowerInvariant().Trim();
            if (dto.IsActive.HasValue) tenant.IsActive = dto.IsActive.Value;
            if (dto.LogoUrl is not null) tenant.LogoUrl = dto.LogoUrl;
            if (dto.PrimaryColor is not null) tenant.PrimaryColor = dto.PrimaryColor;
            if (dto.SecondaryColor is not null) tenant.SecondaryColor = dto.SecondaryColor;
            if (dto.ContactPhone is not null) tenant.ContactPhone = dto.ContactPhone;
            if (dto.ContactEmail is not null) tenant.ContactEmail = dto.ContactEmail;
            if (dto.ThemeMode is not null) tenant.ThemeMode = dto.ThemeMode;

            tenant.UpdatedAt = DateTimeOffset.UtcNow;

            _tenantRepository.Update(tenant);
            await _unitOfWork.SaveChangesAsync();

            return tenant;
        }

        public async Task<Tenant> UpdateLogoAsync(Guid id, string logoUrl)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Tenant '{id}' not found.");

            tenant.LogoUrl = logoUrl;
            tenant.UpdatedAt = DateTimeOffset.UtcNow;

            _tenantRepository.Update(tenant);
            await _unitOfWork.SaveChangesAsync();

            return tenant;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant is null) return false;

            tenant.IsDeleted = true;
            tenant.DeletedAt = DateTimeOffset.UtcNow;
            tenant.IsActive = false;

            _tenantRepository.Update(tenant);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
