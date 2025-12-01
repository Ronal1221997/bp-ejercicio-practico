using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly BankingDbContext _context;

        public CustomerRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            // No olvides el .Include(c => c.Person)
            var query = _context.Customers
                .Include(c => c.Person)
                .OrderBy(c => c.CustomerId);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
