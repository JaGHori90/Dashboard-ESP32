using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public interface IMeasurmentRepository
    {
        Task<List<Measurement>> GetAllAsync();
        Task<Measurement?> GetByIdAsync(int id);
        void Insert(Measurement newMeasurment);
    }
}
