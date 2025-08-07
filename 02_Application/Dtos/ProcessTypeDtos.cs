namespace _02_Application.Dtos;

public abstract record BaseProcessTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public int IconMultiple { get; init; }
    public int IconSingle { get; init; }
    public string ColorBack { get; init; } = string.Empty;
    public string ColorFore { get; init; } = string.Empty;
    public long Target { get; init; }
    public int SortBy { get; init; }
}

public record ProcessTypeDto : BaseProcessTypeDto
{
    public List<ProcessTypeModuleDto> ListModules { get; init; } = [];
    public List<ProcessTypeItemDto> ListItems { get; init; } = [];
}
public record ProcessTypeListDto : BaseProcessTypeDto
{
    public int ModuleCount { get; init; }
    public int ItemCount { get; init; }
    public int ProtocolCount { get; init; }
}
public record ProcessTypeSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ProcessTypeItemDto
{
    public Guid ItemId { get; init; }
    public string ItemName { get; init; } = string.Empty;
}
public record ProcessTypeModuleDto
{
    public Guid ModuleId { get; init; }
    public string ModuleName { get; init; } = string.Empty;
}
