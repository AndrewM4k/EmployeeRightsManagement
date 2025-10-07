using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Repositories
{
    public interface IRightRepository
    {
        IQueryable<Right> Query();
        Task<Right?> FindByIdAsync(int id);
        Task AddAsync(Right right);
        void Update(Right right);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}




