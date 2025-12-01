using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface IPersonRepository
    {
        Task<(IEnumerable<Person> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<Person?> GetByIdAsync(int id);
        Task AddAsync(Person person);
        Task UpdateAsync(Person person);
        Task DeleteAsync(Person person);
    }
}
