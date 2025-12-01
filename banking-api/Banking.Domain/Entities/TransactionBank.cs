using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Domain.Entities
{

    [Table("Transaction")]
    public class TransactionBank
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(30)]
        [Column("transaction_type")]
        public string TransactionType { get; set; } = string.Empty;

        [Column("amount", TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Column("balance", TypeName = "decimal(10, 2)")]
        public decimal Balance { get; set; }

        // Clave foránea
        [Column("account_number")]
        public int AccountNumber { get; set; }

        // Navegación hacia Cuenta
        [ForeignKey("AccountNumber")]
        public virtual Account Account { get; set; } = null!;
    }


}
