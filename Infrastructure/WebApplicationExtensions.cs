using EmployeeRightsManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRightsManagement.Infrastructure
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await db.Database.MigrateAsync();
            DatabaseSeeder.Seed(db);
            return app;
        }
    }
}




