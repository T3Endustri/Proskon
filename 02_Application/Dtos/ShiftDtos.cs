namespace _02_Application.Dtos;
 
public abstract record BaseShiftTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<ShiftTypeDayDto> ListDays { get; init; } = [];
    public List<ShiftTypeLocationDto> ListLocations { get; init; } = [];
}
public record ShiftTypeDto : BaseShiftTypeDto;
public record ShiftTypeListDto : BaseShiftTypeDto
{
    public int DayCount { get; init; }
    public int LocationCount { get; init; }
}
public record ShiftTypeSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ShiftTypeDayDto
{
    public Guid Id { get; init; }
    public byte DayOfWeek { get; init; }
    public long StartTime { get; init; }
    public long EndTime { get; init; }

    public List<ShiftTypeBreakDto> ListBreaks { get; init; } = [];
}
public record ShiftTypeBreakDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public long StartTime { get; init; }
    public long EndTime { get; init; }
}
public record ShiftTypeLocationDto
{
    public Guid ShiftTypeId { get; init; }
    public string ShiftTypeName { get; init; } = string.Empty;

    public Guid LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;
}

public abstract record BaseShiftDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime Start { get; init; }
    public DateTime? End { get; init; }
    public DateTime Finish { get; init; }
    public int Target { get; init; }
}

public record ShiftDto : BaseShiftDto
{
    public Guid LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;
    public List<ShiftBreakDto> Breaks { get; init; } = [];
}
public record ShiftListDto : BaseShiftDto
{
    public string LocationName { get; init; } = string.Empty;
    public int BreakCount { get; init; }
}
public record ShiftSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ShiftBreakDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
