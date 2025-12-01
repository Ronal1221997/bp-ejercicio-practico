using Banking.Application.DTOs;
using Banking.Application.DTOs.ReportDtos;
using Banking.Application.DTOs.TransactionBankDtos;
using Banking.Application.Wrappers;

namespace Banking.Application.Interfaces
{

    public interface ITransactionBankService
    {
        // El servicio recibe un DTO y devuelve un DTO. 
        // La capa de presentación (API) no toca Entidades.
        Task<TransactionBankResponseDto> CreateTransactionAsync(TransactionBankRequestDto request);

        // Podrías agregar otros métodos aquí, como obtener historial mapeado
        Task<PagedResponse<TransactionBankResponseDto>> GetPagedTransactionsAsync(int accountNumber, PaginationFilter filter);




        Task<ReporteFinalDto> GetReportePorFechasAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);

        Task<PagedResponse<TransactionBankResponseDto>> GetAllTransactionsPagedAsync(PaginationFilter filter);

        // Agregar firmas
        Task UpdateTransactionAsync(int id, TransactionBankUpdateDto request);
        Task DeleteTransactionAsync(int id);


    }
}
