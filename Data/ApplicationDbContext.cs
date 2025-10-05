using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public new DbSet<Role> Roles { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<RoleRight> RoleRights { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure EmployeeRole composite key
            builder.Entity<EmployeeRole>()
                .HasKey(er => new { er.EmployeeId, er.RoleId });

            builder.Entity<EmployeeRole>()
                .HasOne(er => er.Employee)
                .WithMany(e => e.EmployeeRoles)
                .HasForeignKey(er => er.EmployeeId);

            builder.Entity<EmployeeRole>()
                .HasOne(er => er.Role)
                .WithMany(r => r.EmployeeRoles)
                .HasForeignKey(er => er.RoleId);

            // Configure RoleRight composite key
            builder.Entity<RoleRight>()
                .HasKey(rr => new { rr.RoleId, rr.RightId });

            builder.Entity<RoleRight>()
                .HasOne(rr => rr.Role)
                .WithMany(r => r.RoleRights)
                .HasForeignKey(rr => rr.RoleId);

            builder.Entity<RoleRight>()
                .HasOne(rr => rr.Right)
                .WithMany(rt => rt.RoleRights)
                .HasForeignKey(rr => rr.RightId);

            // Configure ApplicationUser relationship
            builder.Entity<ApplicationUser>()
                .HasOne(au => au.Employee)
                .WithMany()
                .HasForeignKey(au => au.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
