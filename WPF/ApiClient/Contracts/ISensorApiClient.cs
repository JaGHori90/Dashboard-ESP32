using ApiClient.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.Contracts
{
    public interface ISensorApiClient
    {
        Task<IEnumerable<SensorDto>> GetAllAsync();
    }
}
