using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly BankingDbContext _context;

        public PersonRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Person> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.People.OrderBy(p => p.PersonId); // Ordenar por ID

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _context.People.FindAsync(id);
        }

        public async Task AddAsync(Person person)
        {
            await _context.People.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Person person)
        {
            _context.People.Update(person);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Person person)
        {
            _context.People.Remove(person);
            await _context.SaveChangesAsync();
        }
    }
}
