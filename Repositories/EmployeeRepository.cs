using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRightsManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Employee> Query()
        {
            return _dbContext.Employees.AsQueryable();
        }

        public Task<Employee?> FindByIdAsync(int id)
        {
            return _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task AddAsync(Employee employee)
        {
            return _dbContext.Employees.AddAsync(employee).AsTask();
        }

        public void Update(Employee employee)
        {
            _dbContext.Employees.Update(employee);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}




