using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeeRightsManagement.DTOs;

namespace EmployeeRightsManagement.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRoleRepository _roleRepository;
        private readonly IRightRepository _rightRepository;
        private readonly IMapper _mapper;

        // ReSharper disable once ConvertToPrimaryConstructor
        public RoleService(ApplicationDbContext dbContext, IRoleRepository roleRepository, IRightRepository rightRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _roleRepository = roleRepository;
            _rightRepository = rightRepository;
            _mapper = mapper;
        }

        public async Task<List<object>> GetRolesAsync()
        {
            return await _roleRepository.Query()
                .Where(r => r.IsActive)
                .ProjectTo<RoleListDto>(_mapper.ConfigurationProvider)
                .Select(r => (object)r)
                .ToListAsync();
        }

        public async Task<object?> GetRoleAsync(int id)
        {
            return await _roleRepository.Query()
                .Include(r => r.RoleRights.Where(rr => rr.IsActive))
                    .ThenInclude(rr => rr.Right)
                .Where(r => r.Id == id)
                .ProjectTo<RoleDetailsDto>(_mapper.ConfigurationProvider)
                .Cast<object?>()
                .FirstOrDefaultAsync();
        }

        public async Task<(bool success, string message, int roleId)> CreateRoleAsync(object dto)
        {
            if (dto is not Role role) return (false, "Invalid data", 0);
            role.CreatedDate = DateTime.Now;
            role.IsActive = true;
            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();
            return (true, "Role created successfully", role.Id);
        }

        public async Task<(bool success, string message)> UpdateRoleAsync(object dto)
        {
            if (dto is not Role role) return (false, "Invalid data");
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
            return (true, "Role updated successfully");
        }

        public async Task<(bool success, string message)> DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.FindByIdAsync(id);
            if (role == null) return (false, "Role not found");
            role.IsActive = false;
            await _roleRepository.SaveChangesAsync();
            return (true, "Role deleted successfully");
        }

        public async Task<(bool success, string message)> AssignRightsAsync(int roleId, IEnumerable<int> rightIds)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Get existing active role rights
                var existing = await _dbContext.RoleRights
                    .Where(rr => rr.RoleId == roleId && rr.IsActive)
                    .ToListAsync();
                
                // Mark existing as inactive instead of deleting
                foreach (var existingRight in existing)
                {
                    existingRight.IsActive = false;
                }

                // Get the list of right IDs to add
                var rightIdsList = rightIds.ToList();
                
                // Check if any of the rights to add already exist (even if inactive)
                var existingRights = await _dbContext.RoleRights
                    .Where(rr => rr.RoleId == roleId && rightIdsList.Contains(rr.RightId))
                    .ToListAsync();

                // Reactivate existing rights that are being reassigned
                foreach (var existingRight in existingRights)
                {
                    if (rightIdsList.Contains(existingRight.RightId))
                    {
                        existingRight.IsActive = true;
                        existingRight.AssignedDate = DateTime.Now;
                        rightIdsList.Remove(existingRight.RightId); // Remove from list to avoid duplicates
                    }
                }

                // Add only new role rights (those that don't already exist)
                var toAdd = rightIdsList.Select(rid => new RoleRight
                {
                    RoleId = roleId,
                    RightId = rid,
                    AssignedDate = DateTime.Now,
                    IsActive = true
                });
                
                if (toAdd.Any())
                {
                    await _dbContext.RoleRights.AddRangeAsync(toAdd);
                }
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, "Rights assigned successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error assigning rights: {ex.Message}");
            }
        }

        public async Task<List<object>> GetAllRightsAsync()
        {
            return await _rightRepository.Query()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Category)
                .ThenBy(r => r.Name)
                .ProjectTo<RightListDto>(_mapper.ConfigurationProvider)
                .Select(r => (object)r)
                .ToListAsync();
        }
    }
}


