namespace _02_Application.Dtos;
 
public abstract record BaseModuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PageText { get; init; } = string.Empty;
    public bool IsCanPage { get; init; }
    public bool IsCanShift { get; init; }
    public bool IsCanFilter { get; init; }
    public bool IsCanTemplate { get; init; }
    public bool IsCanBarcode { get; init; }
    public bool IsCanModuleType { get; init; }
    public bool IsCanTarget { get; init; }
    public bool IsCanSerial { get; init; }

    public string ColorBack { get; init; } = string.Empty;
    public string ColorFore { get; init; } = string.Empty;
    public int SortBy { get; init; }
    public List<ModuleHierarchyDto> ListParents { get; init; } = [];
    public List<ModuleHierarchyDto> ListChilds { get; init; } = [];
}

public record ModuleDto : BaseModuleDto;
public record ModuleListDto : BaseModuleDto
{
    public int ParentCount { get; init; }
    public int ChildCount { get; init; }
}
public record ModuleTreeDto : BaseModuleDto
{
    public int Level { get; init; }
    public List<ModuleTreeDto> Children { get; init; } = [];
}
public record ModuleHierarchyDto
{
    public Guid ParentId { get; init; }
    public string ParentName { get; init; } = string.Empty;

    public Guid ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
}