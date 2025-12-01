namespace Banking.Application.DTOs.ReportDtos
{
    public class ReportResponseDto
    {
        // Datos del JSON
        public StatementDto? Data { get; set; }
        // El PDF en formato Base64
        public string Base64Pdf { get; set; } = string.Empty;
    }

    public class StatementDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<AccountReportDto> Accounts { get; set; } = new();
    }

    public class AccountReportDto
    {
        public int AccountNumber { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; } // Saldo actual real en BD
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public List<TransactionReportDetailDto> Transactions { get; set; } = new();
    }

    public class TransactionReportDetailDto
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; } // Se mostrará negativo si es débito
        public decimal Balance { get; set; }
    }

}
