using App.Domain.Entities;

namespace App.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<Users>
{
    Task<Users?> GetByEmailAsync(string email);

    //dapper
    Task<IEnumerable<Users>> GetAllAsync();
}