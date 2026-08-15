using AssignmentSystem.Domain.Enums;

namespace AssignmentSystem.Application.DTOs;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}
