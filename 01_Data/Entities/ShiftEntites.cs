using _01_Data.Entities.Base; 

namespace _01_Data.Entities;

public partial class T3Shift() : BaseEntity
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public DateTime Finish { get; set; }
    public int Target { get; set; } = 0;
    public T3Location Location { get; set; } = default!;
    public List<T3ShiftBreak> ListBreaks { get; set; } = [];
}
public partial class T3ShiftBreak() : BaseEntity
{
    public Guid ShiftId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public T3Shift Shift { get; set; } = default!;
}
public partial class T3ShiftType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<T3ShiftTypeDay> ListDays { get; set; } = [];
    public ICollection<T3ShiftTypeLocation> ListLocations { get; set; } = [];
}
public partial class T3ShiftTypeBreak : BaseEntity
{
    public Guid ShiftTypeDayId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public T3ShiftTypeDay ShiftTypeDay { get; set; } = null!;
}
public partial class T3ShiftTypeCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
public partial class T3ShiftTypeDay : BaseEntity
{
    public Guid ShiftTypeId { get; set; }
    public byte DayOfWeek { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public T3ShiftType ShiftType { get; set; } = null!;
    public ICollection<T3ShiftTypeBreak> T3ShiftTypeBreaks { get; set; } = [];
}
public partial class T3ShiftTypeLocation : BaseEntity
{
    public Guid ShiftTypeId { get; set; }
    public Guid LocationId { get; set; }
    public int Value { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
    public T3Location Location { get; set; } = null!;
    public T3ShiftType ShiftType { get; set; } = null!;

}