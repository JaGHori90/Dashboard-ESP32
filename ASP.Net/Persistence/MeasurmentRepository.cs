using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class MeasurmentRepository : IMeasurmentRepository
    {
        private ApplicationDbContext dbCondtext;

        public MeasurmentRepository(ApplicationDbContext dbCondtext)
        {
            this.dbCondtext = dbCondtext;
        }

        public async Task<List<Measurement>> GetAllAsync()
        {
            return await dbCondtext.Measurements.OrderBy(o=>o.MeasuredAt).ToListAsync();
        }

        public async Task<Measurement?> GetByIdAsync(int id)
        {
            return await dbCondtext.Measurements.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }

        public void Insert(Measurement newMeasurment)
        {
            dbCondtext.Measurements.AddAsync(newMeasurment);
        }
    }
}