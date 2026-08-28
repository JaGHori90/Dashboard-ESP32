using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _dbContext;
        public UnitOfWork() : this(new ApplicationDbContext())
        { }

        public UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = context;
            EmployeeRepository = new EmployeeRepository(_dbContext);
            ActivityRepository = new ActivityRepository(_dbContext);
        }

        public UnitOfWork(IConfiguration configuration) : this(new ApplicationDbContext(configuration))
        { }


        public IEmployeeRepository EmployeeRepository { get; }
        public IActivityRepository ActivityRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            var entities = _dbContext!.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added
                                 || entity.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();  // Geänderte Entities ermitteln

            // Allfällige Validierungen der geänderten Entities durchführen
            foreach (var entity in entities)
            {
                ValidateEntity(entity);
            }
            return await _dbContext.SaveChangesAsync();

       }

        private void ValidateEntity(object entity)
        {       
      
        }

        public async Task DeleteDatabaseAsync() => await _dbContext!.Database.EnsureDeletedAsync();
        public async Task MigrateDatabaseAsync() => await _dbContext!.Database.MigrateAsync();
        public async Task CreateDatabaseAsync() => await _dbContext!.Database.EnsureCreatedAsync();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {   
                if (disposing)
                {
                    await _dbContext.DisposeAsync();
                }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task FillDbAsync()
        {
            await DeleteDatabaseAsync();
            await MigrateDatabaseAsync();

            Sensor emp1 = new Sensor() { FirstName = "Max", LastName = "Mustermann" };
            Sensor emp2 = new Sensor() { FirstName = "Sarah", LastName = "Aigner" };
            _dbContext.Employees.Add(emp1);
            _dbContext.Employees.Add(emp2);

            _dbContext.Activities.Add(new Activity() { ActivityText = "Vorbereitung Schulung", Date = Convert.ToDateTime("10.11.2022"), StartTime = DateTime.Parse("01.01.1900 12:00:00"), EndTime = DateTime.Parse("01.01.1900 14:00:00"), Employee = emp2 });
            _dbContext.Activities.Add(new Activity() { ActivityText = "Durchführung Schulung", Date = Convert.ToDateTime("10.11.2022"), StartTime = DateTime.Parse("01.01.1900 14:00:00"), EndTime = DateTime.Parse("01.01.1900 17:00:00"), Employee = emp2 });
            _dbContext.Activities.Add(new Activity() { ActivityText = "Kundengespräche", Date = Convert.ToDateTime("11.11.2022"), StartTime = DateTime.Parse("01.01.1900 09:15:00"), EndTime = DateTime.Parse("01.01.1900 10:00:00"), Employee = emp2 });
            _dbContext.Activities.Add(new Activity() { ActivityText = "Implementierung Grid Personen", Date = Convert.ToDateTime("11.11.2022"), StartTime = DateTime.Parse("01.01.1900 10:00:00"), EndTime = DateTime.Parse("01.01.1900 15:00:00"), Employee = emp2 });

            await SaveChangesAsync();
        }
    }
}
