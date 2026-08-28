using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Core.Entities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Persistence
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Sensor> Sensor => Set<Sensor>();
        public DbSet<Measurement> Measurements => Set<Measurement>();

        IConfiguration _config;

        public IConfiguration Configuration { get { return _config; } }

        public ApplicationDbContext()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json",optional: true, reloadOnChange:true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional:true, reloadOnChange:true);

            _config = builder.Build();
        }

        public ApplicationDbContext(IConfiguration configuration)
        {
            _config = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            optionsBuilder.UseNpgsql(connectionString);
        }

    }
}
