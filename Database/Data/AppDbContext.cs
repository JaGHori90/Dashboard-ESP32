using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    }
}
