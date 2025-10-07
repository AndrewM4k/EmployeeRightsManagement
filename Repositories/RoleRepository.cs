using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRightsManagement.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Role> Query()
        {
            return _dbContext.Roles.AsQueryable();
        }

        public Task<Role?> FindByIdAsync(int id)
        {
            return _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task AddAsync(Role role)
        {
            return _dbContext.Roles.AddAsync(role).AsTask();
        }

        public void Update(Role role)
        {
            _dbContext.Roles.Update(role);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}




