using AutoMapper;
using Ecosystem.SmartBox.Entities;
using Ecosystem.SmartBox.Users;

namespace Ecosystem.SmartBox;

public class SmartBoxApplicationAutoMapperProfile : Profile 
{
    public SmartBoxApplicationAutoMapperProfile()
    {
        // SmartBoxRole mappings - chỉ map từ Entity sang DTO
        CreateMap<SmartBoxRole, SmartBoxRoleDto>();
        
        // Explicitly define reverse mapping để tránh AutoMapper tự động tạo và gây lỗi
        CreateMap<SmartBoxRoleDto, SmartBoxRole>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.IsSystem, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore())
            .ForMember(x => x.DeleterId, opt => opt.Ignore())
            .ForMember(x => x.DeletionTime, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore());
        
        // Map từ CreateUpdateDto sang Entity (cần thiết cho ABP AutoMapper)
        CreateMap<CreateUpdateSmartBoxRoleDto, SmartBoxRole>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.IsSystem, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore())
            .ForMember(x => x.DeleterId, opt => opt.Ignore())
            .ForMember(x => x.DeletionTime, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore());
            
        // SmartBoxUser mappings
        CreateMap<SmartBoxUser, SmartBoxUserDto>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles sẽ được load riêng
        
        CreateMap<SmartBoxUserDto, SmartBoxUser>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.AuthUserId, opt => opt.Ignore())
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore())
            .ForMember(x => x.DeleterId, opt => opt.Ignore())
            .ForMember(x => x.DeletionTime, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(x => x.Company, opt => opt.Ignore()); // Company navigation property
            
        CreateMap<CreateUpdateSmartBoxUserDto, SmartBoxUser>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.AuthUserId, opt => opt.Ignore())
            .ForMember(x => x.UserName, opt => opt.Ignore())
            .ForMember(x => x.Email, opt => opt.Ignore())
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore())
            .ForMember(x => x.DeleterId, opt => opt.Ignore())
            .ForMember(x => x.DeletionTime, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(x => x.LastLoginTime, opt => opt.Ignore())
            .ForMember(x => x.Company, opt => opt.Ignore()); // Company navigation property
            
        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CompanyDto, Company>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.TenantId, opt => opt.Ignore())
            .ForMember(x => x.CreationTime, opt => opt.Ignore())
            .ForMember(x => x.CreatorId, opt => opt.Ignore())
            .ForMember(x => x.LastModificationTime, opt => opt.Ignore())
            .ForMember(x => x.LastModifierId, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore())
            .ForMember(x => x.DeleterId, opt => opt.Ignore())
            .ForMember(x => x.DeletionTime, opt => opt.Ignore())
            .ForMember(x => x.ExtraProperties, opt => opt.Ignore())
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Ignore());
    }
}
