using Core.Contracts;
using Core.Entities;
using Persistence;

Console.WriteLine("Datenbank wird gelöscht und neu erstellt.....\n");
using (IUnitOfWork uow = new UnitOfWork())
{
    await uow.FillDbAsync();
    List<Sensor> employees = await uow.EmployeeRepository.GetAllAsync();

    foreach (var emp in employees)
    {
        Console.WriteLine($"{emp.FirstName,-15} {emp.LastName,-15}");
    }
}