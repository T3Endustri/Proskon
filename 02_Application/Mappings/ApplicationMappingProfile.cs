using _01_Data.Entities;
using _02_Application.Dtos; 
using AutoMapper;

namespace _02_Application.Mappings;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        // USER
        CreateMap<T3IdentityUser, BaseUserDto>();
        CreateMap<T3IdentityUser, UserDto>().IncludeBase<T3IdentityUser, BaseUserDto>(); 
        
        // ROLE
        CreateMap<T3IdentityRole, BaseRoleDto>();
        CreateMap<T3IdentityRole, RoleDto>().IncludeBase<T3IdentityRole, BaseRoleDto>();
        CreateMap<T3IdentityRole, RoleListDto>().IncludeBase<T3IdentityRole, BaseRoleDto>();
        CreateMap<T3IdentityRole, RoleTreeDto>().IncludeBase<T3IdentityRole, BaseRoleDto>();
        
        // CLAIM
        CreateMap<T3IdentityClaim, BaseClaimDto>();
        CreateMap<T3IdentityClaim, ClaimDto>().IncludeBase<T3IdentityClaim, BaseClaimDto>();

        // LOCATION
        CreateMap<T3Location, BaseLocationDto>();
        CreateMap<T3Location, LocationDto>()
            .IncludeBase<T3Location, BaseLocationDto>()
            .ForMember(d => d.ListParents, o => o.MapFrom(s => s.ListParents))
            .ForMember(d => d.ListChilds, o => o.MapFrom(s => s.ListChilds));
        CreateMap<T3Location, LocationListDto>().IncludeBase<T3Location, BaseLocationDto>();
        CreateMap<T3Location, LocationTreeDto>().IncludeBase<T3Location, BaseLocationDto>();
        CreateMap<T3LocationHierarchy, LocationHierarchyDto>()
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent.Name))
            .ForMember(d => d.ChildName, o => o.MapFrom(s => s.Child.Name));
        
        // ITEM
        CreateMap<T3Item, BaseItemDto>();
        CreateMap<T3Item, ItemDto>()
            .IncludeBase<T3Item, BaseItemDto>()
            .ForMember(d => d.ListParents, o => o.MapFrom(s => s.ListParents))
            .ForMember(d => d.ListChilds, o => o.MapFrom(s => s.ListChilds));
        CreateMap<T3Item, ItemListDto>().IncludeBase<T3Item, BaseItemDto>();
        CreateMap<T3ItemHierarchy, ItemHierarchyDto>()
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent.Name))
            .ForMember(d => d.ChildName, o => o.MapFrom(s => s.Child.Name));

        // MODULE
        CreateMap<T3Module, BaseModuleDto>();
        CreateMap<T3Module, ModuleDto>()
            .IncludeBase<T3Module, BaseModuleDto>()
            .ForMember(d => d.ListParents, o => o.MapFrom(s => s.ListParents))
            .ForMember(d => d.ListChilds, o => o.MapFrom(s => s.ListChilds));
        CreateMap<T3Module, ModuleListDto>().IncludeBase<T3Module, BaseModuleDto>();
        CreateMap<T3ModuleHierarchy, ModuleHierarchyDto>()
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent.Name))
            .ForMember(d => d.ChildName, o => o.MapFrom(s => s.Child.Name));

        // SHIFT
        CreateMap<T3Shift, BaseShiftDto>();
        CreateMap<T3Shift, ShiftDto>().IncludeBase<T3Shift, BaseShiftDto>();
        CreateMap<T3Shift, ShiftListDto>().IncludeBase<T3Shift, BaseShiftDto>();
        CreateMap<T3ShiftBreak, ShiftBreakDto>();

        // SHIFT TYPE
        CreateMap<T3ShiftType, BaseShiftTypeDto>();
        CreateMap<T3ShiftType, ShiftTypeDto>()
            .IncludeBase<T3ShiftType, BaseShiftTypeDto>()
            .ForMember(d => d.ListDays, o => o.MapFrom(s => s.ListDays))
            .ForMember(d => d.ListLocations, o => o.MapFrom(s => s.ListLocations));
        CreateMap<T3ShiftTypeDay, ShiftTypeDayDto>();
        CreateMap<T3ShiftTypeBreak, ShiftTypeBreakDto>();
        CreateMap<T3ShiftTypeLocation, ShiftTypeLocationDto>()
            .ForMember(d => d.ShiftTypeName, o => o.MapFrom(s => s.ShiftType.Name))
            .ForMember(d => d.LocationName, o => o.MapFrom(s => s.Location.Name));

        // TEMPLATE
        CreateMap<T3Template, BaseTemplateDto>();
        CreateMap<T3Template, TemplateDto>()
            .IncludeBase<T3Template, BaseTemplateDto>()
            .ForMember(d => d.ListPanels, o => o.MapFrom(s => s.ListPanels))
            .ForMember(d => d.ListApprovers, o => o.MapFrom(s => s.ListApprovers))
            .ForMember(d => d.ListProperties, o => o.MapFrom(s => s.ListPropertyFields));
        CreateMap<T3Template, TemplateListDto>().IncludeBase<T3Template, BaseTemplateDto>();
        CreateMap<T3TemplatePanel, TemplatePanelDto>();
        CreateMap<T3TemplateApprover, TemplateApproverDto>();

        // PROPERTY
        CreateMap<T3Property, BasePropertyDto>();
        CreateMap<T3Property, PropertyDto>()
            .IncludeBase<T3Property, BasePropertyDto>()
            .ForMember(d => d.ListPanels, o => o.MapFrom(s => s.ListPanels))
            .ForMember(d => d.ListTemplates, o => o.MapFrom(s => s.ListTemplates));
        CreateMap<T3PropertyPanel, PropertyPanelDto>();
        CreateMap<T3PropertyTemplate, PropertyTemplateDto>();

        // PROTOCOL
        CreateMap<T3Protocol, BaseProtocolDto>();
        CreateMap<T3Protocol, ProtocolDto>()
            .IncludeBase<T3Protocol, BaseProtocolDto>()
            .ForMember(d => d.ListProtocolItems, o => o.MapFrom(s => s.ListProtocolItems));
        CreateMap<T3ProtocolItem, ProtocolItemDto>();

        // PROCESS TYPE
        CreateMap<T3ProcessType, BaseProcessTypeDto>();
        CreateMap<T3ProcessType, ProcessTypeDto>()
            .IncludeBase<T3ProcessType, BaseProcessTypeDto>()
            .ForMember(d => d.ListModules, o => o.MapFrom(s => s.ListModules))
            .ForMember(d => d.ListItems, o => o.MapFrom(s => s.ListItems));
        CreateMap<T3ProcessTypeModule, ProcessTypeModuleDto>();
        CreateMap<T3ProcessTypeItem, ProcessTypeItemDto>();

        // FORM
        CreateMap<T3Form, BaseFormDto>();
        CreateMap<T3Form, FormDto>().IncludeBase<T3Form, BaseFormDto>();
        CreateMap<T3Form, FormListDto>().IncludeBase<T3Form, BaseFormDto>();
        CreateMap<T3FormField, FormFieldDto>();
        CreateMap<T3FormFieldValue, FormFieldValueDto>();

        // FORM RESOURCE
        CreateMap<T3FormResource, FormResourceDto>();
        CreateMap<T3FormResourceItem, FormResourceItemDto>();


        CreateMap<T3IdentityUser, UserDto>().ReverseMap(); 
        CreateMap<T3IdentityUser, UserChangePasswordDto>().ReverseMap();

        CreateMap<T3IdentityRole, RoleDto>().ReverseMap(); 

        CreateMap<T3IdentityClaim, ClaimCreateDto>().ReverseMap();

        CreateMap<T3Shift, ShiftDto>().ReverseMap();
        CreateMap<T3ShiftBreak, ShiftBreakDto>().ReverseMap();

        CreateMap<T3Property, PropertyDto>().ReverseMap();

        CreateMap<T3FormFieldValue, FormFieldValueDto>().ReverseMap();
        CreateMap<T3Protocol, ProtocolDto>().ReverseMap();
        CreateMap<T3ProtocolItem, ProtocolItemDto>().ReverseMap();

    }
}
