using App.Domain.Enums;

namespace App.Application.Dto
{
    public record UserDto(
     string Email,
     string FirstName,
     string LastName,
     string Password,
     UserRole Role,
     UserStatus Status = UserStatus.Active);
}
