using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class SensorRepository: ISensorRepository
    {
        private ApplicationDbContext _dbCondtext;

        public SensorRepository(ApplicationDbContext dbCondtext)
        {
            this._dbCondtext = dbCondtext;
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbCondtext.Sensor.CountAsync();
        }

        public async Task<List<Sensor>> GetAllAsync()
        {
            return await _dbCondtext.Sensor.OrderBy(o=>o.Name).ToListAsync();
        }

        public async Task<Sensor?> GetByIdAsync(int id)
        {
            return await _dbCondtext.Sensor.Include(s=>s.Readings).Where(s=>s.Id==id).SingleOrDefaultAsync();
        }

        public void Insert(Sensor newSensor)
        {
           
             _dbCondtext.Sensor.AddAsync(newSensor);
            
        }

        public void Update(Sensor sensor)
        {
            _dbCondtext.Sensor.Update(sensor);
        }
    }
}