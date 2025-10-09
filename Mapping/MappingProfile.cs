using AutoMapper;
using EmployeeRightsManagement.DTOs;
using EmployeeRightsManagement.Models;

namespace EmployeeRightsManagement.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeListDto>()
                .ForMember(d => d.RolesCount, o => o.MapFrom(s => s.EmployeeRoles.Count(er => er.IsActive)));

            CreateMap<Employee, EmployeeDetailsDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.EmployeeRoles.Where(er => er.IsActive).Select(er => er.Role)));
            CreateMap<Role, RoleBasicDto>();
            CreateMap<Role, EmployeeRoleWithRightsDto>()
                .ForMember(d => d.Rights, o => o.MapFrom(s => s.RoleRights.Where(rr => rr.IsActive).Select(rr => rr.Right)));
            CreateMap<Employee, EmployeeWithRolesDto>()
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.EmployeeRoles.Where(er => er.IsActive).Select(er => er.Role)));
            CreateMap<EmployeeRole, EmployeeRoleDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.RoleId))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Role.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Role.Description))
                .ForMember(d => d.AssignedDate, o => o.MapFrom(s => s.AssignedDate));
            CreateMap<RoleRight, EmployeeRightDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.RightId))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Right.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Right.Description))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Right.Category))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Right.Type));

            CreateMap<Role, RoleListDto>()
                .ForMember(d => d.RightsCount, o => o.MapFrom(s => s.RoleRights.Count(rr => rr.IsActive)));
            CreateMap<Role, RoleDetailsDto>();
            CreateMap<RoleRight, RoleRightDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.RightId))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Right.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Right.Description))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Right.Category))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Right.Type))
                .ForMember(d => d.AssignedDate, o => o.MapFrom(s => s.AssignedDate));

            CreateMap<Right, RightListDto>();
            CreateMap<Right, EmployeeRightDto>();
        }
    }
}


