using Core.Contracts;

namespace Core.Contracts
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IEmployeeRepository EmployeeRepository { get; }
        IActivityRepository ActivityRepository { get; }

        Task<int> SaveChangesAsync();
        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();
        Task CreateDatabaseAsync();

        Task FillDbAsync();
    }
}
