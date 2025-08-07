namespace _02_Application.Dtos;

public abstract record BaseLocationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public int SortBy { get; init; }
    public int OperationNo { get; init; }
    public bool IsStation { get; init; }
    public List<LocationHierarchyDto> ListParents { get; init; } = [];
    public List<LocationHierarchyDto> ListChilds { get; init; } = [];
}

public record LocationDto : BaseLocationDto;

public record LocationListDto : BaseLocationDto
{
    public int ParentCount { get; init; }
    public int ChildCount { get; init; }
    public int ItemCount { get; init; }
}

public record LocationTreeDto : BaseLocationDto
{
    public int Level { get; init; }
    public List<LocationTreeDto> Children { get; init; } = [];
}

public record LocationHierarchyDto
{
    public Guid ParentId { get; init; }
    public string ParentName { get; init; } = string.Empty;

    public Guid ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
}