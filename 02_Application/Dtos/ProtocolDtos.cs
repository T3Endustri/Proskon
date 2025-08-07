namespace _02_Application.Dtos;

public abstract record BaseProtocolDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public long Target { get; init; }
    public int SortBy { get; init; }
}
public record ProtocolDto : BaseProtocolDto
{
    public Guid ProcessTypeId { get; init; }
    public string ProcessTypeName { get; init; } = string.Empty;

    public List<ProtocolItemDto> ListProtocolItems { get; init; } = [];
}
public record ProtocolListDto : BaseProtocolDto
{
    public string ProcessTypeName { get; init; } = string.Empty;
    public int ItemCount { get; init; }
}
public record ProtocolSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ProtocolItemDto
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
    public string ItemName { get; init; } = string.Empty;

    public Guid LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;

    public long Target { get; init; }
}
