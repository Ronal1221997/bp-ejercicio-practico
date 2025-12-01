using Banking.Domain.Entities;
using System.Transactions;

namespace Banking.Domain.Interfaces
{

    public interface ITransactionBankRepository
    {
        // Método para guardar una nueva transacción
        Task AddAsync(TransactionBank transaction);

        // Método para obtener transacciones por número de cuenta (historial)
        Task<(IEnumerable<TransactionBank> Items, int TotalCount)> GetByAccountNumberPagedAsync(
        int accountNumber, int pageNumber, int pageSize);

        // Método para obtener transacciones por rango de fechas (reportes)
        Task<IEnumerable<TransactionBank>> GetByDateRangeAsync(int accountNumber, DateTime startDate, DateTime endDate);

        Task<(IEnumerable<TransactionBank> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);

        Task<IEnumerable<TransactionBank>> GetReportDataAsync(int clienteId, DateTime start, DateTime end);

        Task<TransactionBank?> GetByIdAsync(int id);
        Task UpdateAsync(TransactionBank transaction);
        Task DeleteAsync(TransactionBank transaction);
    }
}
