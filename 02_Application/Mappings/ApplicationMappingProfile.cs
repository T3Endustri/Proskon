using System;
using System.Linq;
using _01_Data.Entities;
using _02_Application.Dtos;
using AutoMapper;

namespace _02_Application.Mappings;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        // ========== USER ==========
        CreateMap<T3IdentityUser, BaseUserDto>();

        CreateMap<T3IdentityUser, UserDto>()
            .IncludeBase<T3IdentityUser, BaseUserDto>();

        CreateMap<T3IdentityUser, UserListDto>()
            .IncludeBase<T3IdentityUser, BaseUserDto>()
            .ForMember(d => d.Roles, o => o.MapFrom(s => s.ListRoles.Select(ur => ur.Role)))
            .ForMember(d => d.Claims, o => o.MapFrom(s => s.ListClaims));

        CreateMap<UserDto, T3IdentityUser>()
            .ForMember(d => d.PasswordHash, o => o.Ignore());

        CreateMap<T3IdentityUser, UserInfoDto>()
            .ForMember(d => d.Roles, o => o.MapFrom(s => s.ListRoles.Select(r => r.Role.Name)))
            .ForMember(d => d.Claims, o => o.MapFrom(s => s.ListClaims.Select(c => c.Value)));

        // ========== ROLE ==========
        CreateMap<T3IdentityRole, BaseRoleDto>();

        CreateMap<T3IdentityRole, RoleDto>()
            .IncludeBase<T3IdentityRole, BaseRoleDto>();

        CreateMap<T3IdentityRole, RoleListDto>()
            .IncludeBase<T3IdentityRole, BaseRoleDto>()
            .ForMember(d => d.ParentCount, o => o.MapFrom(s => s.ListParents.Count))
            .ForMember(d => d.ChildCount, o => o.MapFrom(s => s.ListChilds.Count))
            .ForMember(d => d.UserCount, o => o.MapFrom(s => s.ListUsers.Count))
            // CSV alanları servis tarafında hesaplanacak:
            .ForMember(d => d.ParentCsv, o => o.Ignore())
            .ForMember(d => d.ChildCsv, o => o.Ignore())
            .ForMember(d => d.UserCsv, o => o.Ignore());

        CreateMap<T3IdentityRole, RoleTreeDto>()
            .IncludeBase<T3IdentityRole, BaseRoleDto>()
            .ForMember(d => d.Children, o => o.Ignore());


        // DTO -> Entity
        CreateMap<RoleDto, T3IdentityRole>()
            .ForMember(d => d.Id, o => o.Condition((src, dest, val) => src.Id != Guid.Empty))
            .ForMember(d => d.ListParents, o => o.Ignore())
            .ForMember(d => d.ListChilds, o => o.Ignore())
            .ForMember(d => d.ListUsers, o => o.Ignore())
            .ForMember(d => d.ListClaims, o => o.Ignore())
            .ForMember(d => d.ListApproveTemplates, o => o.Ignore());

        // ========== CLAIM ==========
        CreateMap<T3IdentityClaim, ClaimDto>();
        CreateMap<T3IdentityClaim, ClaimCreateDto>().ReverseMap();

        // ========== LOCATION ==========
        CreateMap<T3Location, BaseLocationDto>();
        CreateMap<T3Location, LocationDto>()
            .IncludeBase<T3Location, BaseLocationDto>();
        CreateMap<T3Location, LocationListDto>()
            .IncludeBase<T3Location, BaseLocationDto>()
            .ForMember(d => d.ParentCount, o => o.MapFrom(s => s.ListParents.Count))
            .ForMember(d => d.ChildCount, o => o.MapFrom(s => s.ListChilds.Count))
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.ListItems.Count));
        CreateMap<T3LocationHierarchy, LocationHierarchyDto>()
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent.Name))
            .ForMember(d => d.ChildName, o => o.MapFrom(s => s.Child.Name));
        CreateMap<T3Location, LocationTreeDto>()
            .IncludeBase<T3Location, BaseLocationDto>()
            .ForMember(d => d.Children, o => o.Ignore());

        // ========== MODULE ==========
        CreateMap<T3Module, BaseModuleDto>();
        CreateMap<T3Module, ModuleDto>()
            .IncludeBase<T3Module, BaseModuleDto>();
        CreateMap<T3Module, ModuleListDto>()
            .IncludeBase<T3Module, BaseModuleDto>()
            .ForMember(d => d.ParentCount, o => o.MapFrom(s => s.ListParents.Count))
            .ForMember(d => d.ChildCount, o => o.MapFrom(s => s.ListChilds.Count));
        CreateMap<T3ModuleHierarchy, ModuleHierarchyDto>()
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent.Name))
            .ForMember(d => d.ChildName, o => o.MapFrom(s => s.Child.Name));
        CreateMap<T3Module, ModuleTreeDto>()
            .IncludeBase<T3Module, BaseModuleDto>()
            .ForMember(d => d.Children, o => o.Ignore());
        CreateMap<ModuleDto, T3Module>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.ListParents, o => o.Ignore())
            .ForMember(d => d.ListChilds, o => o.Ignore())
            .ForMember(d => d.ListItems, o => o.Ignore())
            .ForMember(d => d.ListModuleTypeItems, o => o.Ignore())
            .ForMember(d => d.ListProcessTypes, o => o.Ignore());

        // ========== ITEM ==========
        CreateMap<T3Item, BaseItemDto>();
        CreateMap<T3Item, ItemDto>()
            .IncludeBase<T3Item, BaseItemDto>();
        CreateMap<T3Item, ItemListDto>()
            .IncludeBase<T3Item, BaseItemDto>()
            .ForMember(d => d.ModuleName, o => o.MapFrom(s => s.Module.Name))
            .ForMember(d => d.LocationName, o => o.MapFrom(s => s.Location != null ? s.Location.Name : null))
            .ForMember(d => d.ModuleTypeName, o => o.MapFrom(s => s.ModuleType != null ? s.ModuleType.Name : null));
        CreateMap<T3ItemHierarchy, ItemHierarchyDto>()
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent.Name))
            .ForMember(d => d.ChildName, o => o.MapFrom(s => s.Child.Name));
        CreateMap<T3Item, ItemTreeDto>()
            .IncludeBase<T3Item, BaseItemDto>()
            .ForMember(d => d.Children, o => o.Ignore());

        // ========== PROCESS TYPE ==========
        CreateMap<T3ProcessType, BaseProcessTypeDto>();
        CreateMap<T3ProcessType, ProcessTypeDto>()
            .IncludeBase<T3ProcessType, BaseProcessTypeDto>()
            .ForMember(d => d.ListModules, o => o.MapFrom(s => s.ListModules))
            .ForMember(d => d.ListItems, o => o.MapFrom(s => s.ListItems));

        // *** Buradaki iki satır hatayı çıkarıyordu; generic CreateMap ile düzeltildi ***
        CreateMap<T3ProcessTypeModule, ProcessTypeModuleDto>()
            .ForMember(d => d.ModuleId, o => o.MapFrom(s => s.ModuleId))
            .ForMember(d => d.ModuleName, o => o.MapFrom(s => s.Module.Name));

        CreateMap<T3ProcessTypeItem, ProcessTypeItemDto>()
            .ForMember(d => d.ItemId, o => o.MapFrom(s => s.ItemId))
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item.Name));

        CreateMap<T3ProcessType, ProcessTypeListDto>()
            .IncludeBase<T3ProcessType, BaseProcessTypeDto>()
            .ForMember(d => d.ModuleCount, o => o.MapFrom(s => s.ListModules.Count))
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.ListItems.Count))
            .ForMember(d => d.ProtocolCount, o => o.MapFrom(s => s.ListProtocols.Count));

        // ========== PROTOCOL ==========
        CreateMap<T3Protocol, BaseProtocolDto>();
        CreateMap<T3Protocol, ProtocolDto>()
            .IncludeBase<T3Protocol, BaseProtocolDto>()
            .ForMember(d => d.ProcessTypeName, o => o.MapFrom(s => s.ProcessType.Name))
            .ForMember(d => d.ListProtocolItems, o => o.MapFrom(s => s.ListProtocolItems));
        CreateMap<T3Protocol, ProtocolListDto>()
            .IncludeBase<T3Protocol, BaseProtocolDto>()
            .ForMember(d => d.ProcessTypeName, o => o.MapFrom(s => s.ProcessType.Name))
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.ListProtocolItems.Count));
        CreateMap<T3ProtocolItem, ProtocolItemDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item.Name))
            .ForMember(d => d.LocationName, o => o.MapFrom(s => s.Location.Name));

        // ========== TEMPLATE / PROPERTY / FORM ==========
        CreateMap<T3Template, BaseTemplateDto>();
        CreateMap<T3Template, TemplateDto>()
            .IncludeBase<T3Template, BaseTemplateDto>()
            .ForMember(d => d.ListPanels, o => o.MapFrom(s => s.ListPanels))
            .ForMember(d => d.ListApprovers, o => o.MapFrom(s => s.ListApprovers))
            .ForMember(d => d.ListProperties, o => o.MapFrom(s => s.ListPropertyFields));
        CreateMap<T3Template, TemplateListDto>()
            .IncludeBase<T3Template, BaseTemplateDto>()
            .ForMember(d => d.FormCount, o => o.MapFrom(s => s.ListForms.Count))
            .ForMember(d => d.PanelCount, o => o.MapFrom(s => s.ListPanels.Count))
            .ForMember(d => d.ApproverCount, o => o.MapFrom(s => s.ListApprovers.Count));
        CreateMap<T3TemplatePanel, TemplatePanelDto>();
        CreateMap<T3TemplateApprover, TemplateApproverDto>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.FirstName + " " + s.User.LastName : null))
            .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role != null ? s.Role.Name : null));
        CreateMap<T3PropertyTemplate, PropertyTemplateDto>()
            .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.PropertyField.Name));

        CreateMap<T3Property, BasePropertyDto>();
        CreateMap<T3Property, PropertyDto>()
            .IncludeBase<T3Property, BasePropertyDto>()
            .ForMember(d => d.FormResourceName, o => o.MapFrom(s => s.FormResource != null ? s.FormResource.Name : null))
            .ForMember(d => d.ListPanels, o => o.MapFrom(s => s.ListPanels))
            .ForMember(d => d.ListTemplates, o => o.MapFrom(s => s.ListTemplates));
        CreateMap<T3Property, PropertyListDto>()
            .IncludeBase<T3Property, BasePropertyDto>()
            .ForMember(d => d.FileTypes, o => o.MapFrom(s => s.FileTypes));
        CreateMap<T3PropertyPanel, PropertyPanelDto>();
        CreateMap<T3PropertyTemplate, PropertyTemplateDto>();

        CreateMap<T3Form, BaseFormDto>();
        CreateMap<T3Form, FormDto>()
            .IncludeBase<T3Form, BaseFormDto>()
            .ForMember(d => d.CreateUserName, o => o.MapFrom(s => s.CreateUser.FirstName + " " + s.CreateUser.LastName))
            .ForMember(d => d.ApprovedUserName, o => o.MapFrom(s => s.ApprovedUser != null ? (s.ApprovedUser.FirstName + " " + s.ApprovedUser.LastName) : null))
            .ForMember(d => d.ListFormFields, o => o.MapFrom(s => s.ListFormFields));
        CreateMap<T3Form, FormListDto>()
            .IncludeBase<T3Form, BaseFormDto>()
            .ForMember(d => d.CreateUserName, o => o.MapFrom(s => s.CreateUser.FirstName + " " + s.CreateUser.LastName))
            .ForMember(d => d.ApprovedUserName, o => o.MapFrom(s => s.ApprovedUser != null ? (s.ApprovedUser.FirstName + " " + s.ApprovedUser.LastName) : null))
            .ForMember(d => d.FieldCount, o => o.MapFrom(s => s.ListFormFields.Count));
        CreateMap<T3FormField, FormFieldDto>()
            .ForMember(d => d.PropertyName, o => o.MapFrom(s => s.PropertyField.Name))
            .ForMember(d => d.ListValues, o => o.MapFrom(s => s.ListValues));
        CreateMap<T3FormFieldValue, FormFieldValueDto>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.CreateUser.FirstName + " " + s.CreateUser.LastName));
        CreateMap<T3FormResource, FormResourceDto>();
        CreateMap<T3FormResourceItem, FormResourceItemDto>();
        CreateMap<T3FormResource, FormResourceListDto>();
    }
}
