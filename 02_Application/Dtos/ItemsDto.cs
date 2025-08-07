namespace _02_Application.Dtos;
 
public abstract record BaseItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public string ExternalFilter { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public long Target { get; init; }
    public short Status { get; init; }
    public int SortBy { get; init; }
    public List<ItemHierarchyDto> ListParents { get; init; } = [];
    public List<ItemHierarchyDto> ListChilds { get; init; } = [];
}

public record ItemDto : BaseItemDto
{
    public Guid ModuleId { get; init; }
    public Guid? LocationId { get; init; }
    public Guid? ModuleTypeId { get; init; }
}

public record ItemListDto : BaseItemDto
{
    public string ModuleName { get; init; } = string.Empty;
    public string? LocationName { get; init; }
    public string? ModuleTypeName { get; init; }
}

public record ItemSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public record ItemTreeDto : BaseItemDto
{
    public int Level { get; set; }
    public List<ItemTreeDto> Children { get; set; } = [];
}
public record ItemHierarchyDto
{
    public Guid ParentId { get; init; }
    public string ParentName { get; init; } = string.Empty;

    public Guid ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
}