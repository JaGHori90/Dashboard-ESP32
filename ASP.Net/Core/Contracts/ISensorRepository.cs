using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public interface ISensorRepository
    {
        Task<List<Sensor>> GetAllAsync();
        Task<Sensor?> GetByIdAsync(int id);
        void Insert(Sensor newSensor);
    }
}
