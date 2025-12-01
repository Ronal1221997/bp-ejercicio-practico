using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Account> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            // Include anidado (Account -> Customer -> Person)
            var query = _context.Accounts
                .Include(a => a.Customer)
                .ThenInclude(c => c.Person)
                .OrderBy(a => a.AccountNumber);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Account?> GetByIdAsync(int accountNumber)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .ThenInclude(c => c.Person)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Account account)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Accounts
                .Where(a => a.CustomerId == customerId) // Filtro por Cliente
                .Include(a => a.Customer)               // Opcional: Traer datos del cliente si los necesitas
                .ThenInclude(c => c.Person)             // Opcional: Traer datos de la persona
                .AsNoTracking()                         // <--- OPTIMIZACIÓN CLAVE PARA REPORTES
                .ToListAsync();
        }
    }
}
