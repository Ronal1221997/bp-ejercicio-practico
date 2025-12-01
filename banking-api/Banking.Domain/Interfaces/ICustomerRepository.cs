using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}
