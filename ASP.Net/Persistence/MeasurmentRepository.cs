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
            return await dbCondtext.Measurements.ToListAsync();
        }
    }
}