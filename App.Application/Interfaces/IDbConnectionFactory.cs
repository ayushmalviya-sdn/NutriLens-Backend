using System.Data;

namespace App.Application.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionString, string provider);
    }
}
