namespace Banking.Application.DTOs
{
    public class TransactionBankResponseDto
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; } // El saldo actualizado
        public int AccountNumber { get; set; }
    }
}
