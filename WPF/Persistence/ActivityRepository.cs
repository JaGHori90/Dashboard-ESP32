using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class ActivityRepository : IActivityRepository
    {
        private ApplicationDbContext _dbContext;

        public ActivityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(Activity item)
        {
            _dbContext.Activities.Remove(item);
        }

        public async Task<List<Activity>> GetAllByEmpIdAsync(int employeeId)
        {
            return await _dbContext.Activities.Where(a=>a.Employee_Id==employeeId).OrderBy(a=>a.Date)
                .ThenBy(a=>a.StartTime).ToListAsync();
        }

        public void Insert(Activity activity)
        {
            _dbContext.Activities.Add(activity);
        }

        public void Update(Activity activity)
        {
            _dbContext.Activities.Update(activity);
        }
    }
}