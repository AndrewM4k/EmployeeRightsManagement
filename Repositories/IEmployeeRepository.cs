using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Repositories
{
    public interface IEmployeeRepository
    {
        IQueryable<Employee> Query();
        Task<Employee?> FindByIdAsync(int id);
        Task AddAsync(Employee employee);
        void Update(Employee employee);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}




