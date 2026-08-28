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
            return await dbCondtext.Sensor.ToListAsync();
        }

        public async Task<Sensor?> GetAnySensor()
        {
            return await dbCondtext.Sensor.SingleOrDefaultAsync();
        }
    }
}