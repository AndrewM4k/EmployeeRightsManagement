using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using EmployeeRightsManagement.Repositories;
using EmployeeRightsManagement.ViewModels;
using EmployeeRightsManagement.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRightsManagement.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(ApplicationDbContext dbContext, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeListDto>> GetEmployeesAsync()
        {
            return await _employeeRepository.Query()
                .Where(e => e.IsActive)
                .Include(e => e.EmployeeRoles)
                .ProjectTo<EmployeeListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<EmployeeDetailsDto?> GetEmployeeDetailsAsync(int id)
        {
            return await _employeeRepository.Query()
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                        .ThenInclude(r => r.RoleRights)
                            .ThenInclude(rr => rr.Right)
                .Where(e => e.Id == id)
                .ProjectTo<EmployeeDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<List<RoleListDto>> GetAllRolesAsync()
        {
            return await _dbContext.Roles
                .Where(r => r.IsActive)
                .ProjectTo<RoleListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<(bool success, string message, int employeeId)> CreateEmployeeAsync(object dto)
        {
            if (dto is not Employee employee)
            {
                return (false, "Invalid data", 0);
            }
            employee.CreatedDate = DateTime.Now;
            employee.IsActive = true;
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            return (true, "Employee created successfully", employee.Id);
        }

        public async Task<(bool success, string message)> UpdateEmployeeAsync(object dto)
        {
            if (dto is not Employee employee)
            {
                return (false, "Invalid data");
            }
            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();
            return (true, "Employee updated successfully");
        }

        public async Task<(bool success, string message)> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.FindByIdAsync(id);
            if (employee == null)
            {
                return (false, "Employee not found");
            }
            employee.IsActive = false;
            await _employeeRepository.SaveChangesAsync();
            return (true, "Employee deleted successfully");
        }

        public async Task<(bool success, string message)> AssignRolesAsync(int employeeId, IEnumerable<int> roleIds)
        {
            var existing = await _dbContext.EmployeeRoles
                .Where(er => er.EmployeeId == employeeId)
                .ToListAsync();
            _dbContext.EmployeeRoles.RemoveRange(existing);

            var toAdd = roleIds.Select(roleId => new EmployeeRole
            {
                EmployeeId = employeeId,
                RoleId = roleId,
                AssignedDate = DateTime.Now,
                IsActive = true
            });
            await _dbContext.EmployeeRoles.AddRangeAsync(toAdd);
            await _dbContext.SaveChangesAsync();
            return (true, "Roles assigned successfully");
        }

        public async Task<List<EmployeeWithRolesDto>> SearchEmployeesAsync(string? name, string? role, string? right, int[]? roleIds)
        {
            var query = _employeeRepository.Query()
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(e => e.EmployeeRoles.Any(er => er.IsActive && er.Role.Name.Contains(role)));
            }

            if (roleIds != null && roleIds.Length > 0)
            {
                query = query.Where(e => e.EmployeeRoles.Any(er => er.IsActive && roleIds.Contains(er.RoleId)));
            }

            if (!string.IsNullOrEmpty(right))
            {
                query = query.Where(e => e.EmployeeRoles
                    .Any(er => er.IsActive && er.Role.RoleRights
                        .Any(rr => rr.IsActive && rr.Right.Name.Contains(right))));
            }

            return await query
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                .ProjectTo<EmployeeWithRolesDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}


