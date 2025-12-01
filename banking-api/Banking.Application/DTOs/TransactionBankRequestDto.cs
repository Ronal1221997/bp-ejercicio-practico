using System.ComponentModel.DataAnnotations;

namespace Banking.Application.DTOs
{
    public class TransactionBankRequestDto
    {
        [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
        public int AccountNumber { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "El tipo de transacción es obligatorio.")]
        // Podrías validar que solo acepte "Deposit" o "Withdrawal" aquí o en la lógica
        public string TransactionType { get; set; } = string.Empty;
    }
}
