using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Repositories
{
    public interface IRoleRepository
    {
        IQueryable<Role> Query();
        Task<Role?> FindByIdAsync(int id);
        Task AddAsync(Role role);
        void Update(Role role);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}




