using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VoroBidsCrm.Application.DTOs;
using VoroBidsCrm.Application.Services.Base;
using VoroBidsCrm.Application.Services.Interfaces;
using VoroBidsCrm.Domain.Entities;
using System.Collections.Concurrent;
using VoroBidsCrm.Domain.Enums;
using VoroBidsCrm.Domain.Interfaces.Repositories;
using VoroBidsCrm.Domain.Interfaces.UnitOfWork;

namespace VoroBidsCrm.Application.Services
{
    public class UserExtensionService(IUserExtensionRepository userExtensionRepository, IMapper mapper) : ServiceBase<UserExtension>(userExtensionRepository), IUserExtensionService
    {

        public async Task<UserExtensionDto> CreateAsync(UserExtensionDto dto)
        {
            var createUserExtensionDto = mapper.Map<UserExtension>(dto);

            await base.AddAsync(createUserExtensionDto);

            return mapper.Map<UserExtensionDto>(createUserExtensionDto);
        }

        public Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserExtensionDto>> GetAllAsync()
        {
            var userExtensions = await base.Query()
                .ToListAsync();

            return mapper.Map<IEnumerable<UserExtensionDto>>(userExtensions);
        }

        public async Task<UserExtensionDto?> GetByIdAsync(Guid id)
        {
            var userExtension = await base.Query()
                .Where(s => s.UserId == id)
                .FirstOrDefaultAsync();

            return mapper.Map<UserExtensionDto?>(userExtension);
        }

        public async Task<UserExtensionDto> UpdateAsync(Guid id, UserExtensionDto dto)
        {
            var existingUserExtension = await base.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("UserExtension não encontrado");

            mapper.Map(dto, existingUserExtension);

            base.Update(existingUserExtension);

            return mapper.Map<UserExtensionDto>(existingUserExtension);
        }
    }
}