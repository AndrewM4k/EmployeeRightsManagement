using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeeRightsManagement.DTOs;

namespace EmployeeRightsManagement.Services.Rights
{
    public class RightService : IRightService
    {
        private readonly IRightRepository _rightRepository;
        private readonly IMapper _mapper;

        // ReSharper disable once ConvertToPrimaryConstructor
        public RightService(IRightRepository rightRepository, IMapper mapper)
        {
            _rightRepository = rightRepository;
            _mapper = mapper;
        }

        public async Task<List<object>> GetRightsAsync(string? name, string? category, string? type)
        {
            var query = _rightRepository.Query().Where(r => r.IsActive);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(r => r.Category.Contains(category));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(r => r.Type.Contains(type));
            }

            return await query
                .OrderBy(r => r.Category)
                .ThenBy(r => r.Name)
                .ProjectTo<RightListDto>(_mapper.ConfigurationProvider)
                .Select(r => (object)r)
                .ToListAsync();
        }

        public async Task<(bool success, string message)> CreateRightAsync(object dto)
        {
            if (dto is not Right right) return (false, "Invalid data");
            right.CreatedDate = DateTime.Now;
            right.IsActive = true;
            await _rightRepository.AddAsync(right);
            await _rightRepository.SaveChangesAsync();
            return (true, "Right created successfully");
        }

        public async Task<(bool success, string message)> UpdateRightAsync(object dto)
        {
            if (dto is not Right right) return (false, "Invalid data");
            _rightRepository.Update(right);
            await _rightRepository.SaveChangesAsync();
            return (true, "Right updated successfully");
        }

        public async Task<(bool success, string message)> DeleteRightAsync(int id)
        {
            var right = await _rightRepository.FindByIdAsync(id);
            if (right == null) return (false, "Right not found");
            right.IsActive = false;
            await _rightRepository.SaveChangesAsync();
            return (true, "Right deleted successfully");
        }
    }
}


