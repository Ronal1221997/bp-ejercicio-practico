using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence.Repositories
{
    public class TransactionBankRepository : ITransactionBankRepository
    {
        private readonly BankingDbContext _context;

        public TransactionBankRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TransactionBank transaction)
        {
            // Agregamos la entidad al contexto
            await _context.Transactions.AddAsync(transaction);

            // Guardamos los cambios en la BD real
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<TransactionBank> Items, int TotalCount)> GetByAccountNumberPagedAsync(
    int accountNumber, int pageNumber, int pageSize)
        {
            // 1. Preparar la consulta base (sin ejecutarla aún)
            var query = _context.Transactions
                .Where(t => t.AccountNumber == accountNumber)
                .OrderByDescending(t => t.Date); // Importante: Siempre ordenar al paginar

            // 2. Contar el total REAL de registros (antes de cortar la página)
            // Esto hace un "SELECT COUNT(*)"
            var totalCount = await query.CountAsync();

            // 3. Aplicar paginación y ejecutar la consulta
            // Ejemplo: Página 2, tamaño 10 -> Salta los primeros 10, toma los siguientes 10.
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<TransactionBank>> GetByDateRangeAsync(int accountNumber, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.AccountNumber == accountNumber &&
                            t.Date >= startDate &&
                            t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<(IEnumerable<TransactionBank> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            // 1. Consulta base con relaciones necesarias
            // Incluimos Account y Customer por si el DTO necesita mostrar nombre del cliente o tipo de cuenta
            var query = _context.Transactions
                .Include(t => t.Account)
                .ThenInclude(a => a.Customer)
                .ThenInclude(c => c.Person)
                .OrderByDescending(t => t.Date); // Orden cronológico inverso

            // 2. Conteo total (rápido)
            var totalCount = await query.CountAsync();

            // 3. Paginación (Skip/Take)
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<TransactionBank>> GetReportDataAsync(int clienteId, DateTime start, DateTime end)
        {
            // 1. Iniciar la consulta base incluyendo datos de Cuenta, Cliente y Persona
            var query = _context.Transactions
                .Include(t => t.Account)
                .ThenInclude(a => a.Customer)
                .ThenInclude(c => c.Person)
                .AsNoTracking(); // Vital para reportes (solo lectura rápida)

            // 2. APLICAR FILTRO DE FECHAS (Siempre se aplica)
            // Usamos .Date para ignorar la hora si es necesario
            query = query.Where(t => t.Date.Date >= start.Date && t.Date.Date <= end.Date);

            // 3. APLICAR FILTRO DE CLIENTE (Solo si clienteId > 0)
            // Si clienteId es 0, esta línea se salta y trae a TODOS los clientes.
            if (clienteId > 0)
            {
                query = query.Where(t => t.Account.CustomerId == clienteId);
            }

            // 4. Ejecutar y ordenar
            return await query
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }


        public async Task<TransactionBank?> GetByIdAsync(int id)
        {
            // Incluimos la cuenta porque necesitaremos modificar su saldo
            return await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }

        public async Task UpdateAsync(TransactionBank transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TransactionBank transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

    }
}
