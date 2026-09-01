using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class SensorRepository: ISensorRepository
    {
        private ApplicationDbContext dbCondtext;

        public SensorRepository(ApplicationDbContext dbCondtext)
        {
            this.dbCondtext = dbCondtext;
        }

        public async Task<List<Sensor>> GetAllAsync()
        {
            return await dbCondtext.Sensor.OrderBy(o=>o.Name).ToListAsync();
        }

        public async Task<Sensor?> GetByIdAsync(int id)
        {
            return await dbCondtext.Sensor.Where(s=>s.Id==id).SingleOrDefaultAsync();
        }

        public void Insert(Sensor newSensor)
        {
           
             dbCondtext.Sensor.AddAsync(newSensor);
            
        }
    }
}