namespace _02_Application.Dtos;


public abstract record BaseUserDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string PhotoUrl { get; init; } = string.Empty;
}

public record UserListDto : BaseUserDto
{
    public List<RoleDto> Roles { get; init; } = [];
    public List<ClaimDto> Claims { get; init; } = [];
}

public record UserDto : BaseUserDto
{
    public string Password { get; init; } = string.Empty;
}

public record UserChangePasswordDto
{
    public Guid Id { get; init; }
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}


public abstract record BaseRoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string StartPage { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsTeam { get; init; }
    public bool IsDepartment { get; init; }
}
public record RoleDto : BaseRoleDto;
public record RoleListDto : BaseRoleDto
{
    public int ParentCount { get; init; }
    public int ChildCount { get; init; }
    public int UserCount { get; init; }
}
public record RoleTreeDto : BaseRoleDto
{
    public int Level { get; set; }
    public List<RoleTreeDto> Children { get; set; } = [];
}
 
public abstract record BaseClaimDto
{
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public short PermissionType { get; init; }
}

public record ClaimDto : BaseClaimDto
{
    public Guid Id { get; init; }
}

public record ClaimCreateDto : BaseClaimDto
{
    public Guid? UserId { get; init; }
    public Guid? RoleId { get; init; }
}

public record ClaimAssignDto
{
    public Guid? UserId { get; init; }
    public Guid? RoleId { get; init; }
    public List<ClaimCreateDto> Claims { get; init; } = [];
}

public record ClaimGroupDto
{
    public string GroupName { get; init; } = string.Empty;
    public List<ClaimDto> Claims { get; init; } = [];
}
