using App.Application.Dto;
using App.Domain.Entities;

namespace App.Application.Interfaces.Services
{
    public interface IUserService
    {
        public Task<UserDto?> GetUserByUserIdAsync(int userId);
        public Task<int> CreateUserAsync(UserDto userDto);
        public Task<List<Users>> GetAllAsync();
    }
}
