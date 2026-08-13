using Core.Entities;

namespace Core.Contracts
{
    public interface IEmployeeRepository
    {
        Task<List<Sensor>> GetAllAsync();
        Task<Sensor?> GetByIdAsync(int id);
        void Update(Sensor editEmp);
        void Insert(Sensor editEmp);
        void Delete(Sensor employee);
    }
}
