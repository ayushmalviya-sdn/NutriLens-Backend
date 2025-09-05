using App.SharedConfigs.Models;
using Microsoft.EntityFrameworkCore;

namespace App.SharedConfigs.DBContext
{
    public class MasterDbContext(DbContextOptions<MasterDbContext> options) : DbContext(options)
    {
        public DbSet<IndustryConfig> IndustryConfig { get; set; }
    }
}
