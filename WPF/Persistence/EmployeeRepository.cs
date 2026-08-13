using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class EmployeeRepository : IEmployeeRepository
    {
        private ApplicationDbContext _dbContext;

        public EmployeeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Sensor>> GetAllAsync()
        {
            return await _dbContext.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync();
        }

        public async Task<Sensor?> GetByIdAsync(int id)
        {
            return await _dbContext.Employees.SingleOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Löscht Datensatz aus DbSet
        /// (Datensatz muss nicht mehr zuvor extra geladen werden -> State wird auf Deleted gesetzt)
        /// </summary>
        /// <param name="employee"></param>
        public void Delete(Sensor employee)
        {
            _dbContext.Employees.Remove(employee);
        }

        /// <summary>
        /// Fügt neuen Datensatz in DbSet hinzu
        /// </summary>
        /// <param name="editEmp"></param>
        public void Insert(Sensor editEmp)
        {
            _dbContext.Employees.Add(editEmp);
        }

        /// <summary>
        /// Führt Update eines Datensatzes im DbSet durch
        /// (Datensatz muss nicht mehr zuvor geladen werden -> State wird auf Modified gesetzt)
        /// </summary>
        /// <param name="editEmp"></param>
        public void Update(Sensor editEmp)
        {
            _dbContext.Employees.Update(editEmp);
        }
    }
}