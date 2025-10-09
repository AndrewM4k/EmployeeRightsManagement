using EmployeeRightsManagement.Data;
using EmployeeRightsManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRightsManagement.Repositories
{
    public class RightRepository : IRightRepository
    {
        private readonly ApplicationDbContext _dbContext;

        // ReSharper disable once ConvertToPrimaryConstructor
        public RightRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Right> Query()
        {
            return _dbContext.Rights.AsQueryable();
        }

        public Task<Right?> FindByIdAsync(int id)
        {
            return _dbContext.Rights.FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task AddAsync(Right right)
        {
            return _dbContext.Rights.AddAsync(right).AsTask();
        }

        public void Update(Right right)
        {
            _dbContext.Rights.Update(right);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}