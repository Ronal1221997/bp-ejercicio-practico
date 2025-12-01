using Banking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<(IEnumerable<Account> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);
        // Usamos int porque account_number es el PK
        Task<Account?> GetByIdAsync(int accountNumber);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);

        Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId);
    }
}
