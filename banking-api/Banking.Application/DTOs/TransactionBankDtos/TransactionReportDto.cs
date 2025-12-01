using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.DTOs.TransactionBankDtos
{
    public class TransactionReportDto
    {
        public int AccountNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Resumen del periodo
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }

        // Lista de movimientos
        public IEnumerable<TransactionBankResponseDto> Transactions { get; set; } = new List<TransactionBankResponseDto>();
    }
}
