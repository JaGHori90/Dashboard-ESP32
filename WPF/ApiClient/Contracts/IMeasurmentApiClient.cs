using ApiClient.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.Contracts
{
    public interface IMeasurmentApiClient
    {
        Task<IEnumerable<MeasurmentDto>> GetAllAsync();
        Task<int> CleanupAsync();
    }
}
