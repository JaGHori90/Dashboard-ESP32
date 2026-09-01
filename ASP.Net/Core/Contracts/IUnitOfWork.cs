using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public interface IUnitOfWork: IAsyncDisposable, IDisposable
    {
        ISensorRepository SensorRepository { get; }
        IMeasurmentRepository MeasurmentRepository { get; }

        Task<int> SaveChangesAsync();
        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();
        Task CreateDatababaseAsync();
        Task FillDbAsync();
    }
}
