using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Domain.Entities;
using App.Infrastructure.DBContext;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository;

public class UserRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory) : BaseRepository<Users>(context, dbConnectionFactory), IUserRepository
{
    public async Task<IEnumerable<Users>> GetAllAsync()
    {
        using var connection = dbConnection;
        const string sql = "SELECT * FROM Users";
        return await connection.QueryAsync<Users>(sql);
    }

    public async Task<Users?> GetByEmailAsync(string email)
    {
        return await DbSet.SingleOrDefaultAsync(user => user.Email == email);
    }
}