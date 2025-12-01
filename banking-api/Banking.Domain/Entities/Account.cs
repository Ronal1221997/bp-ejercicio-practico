using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Domain.Entities
{
    [Table("Account")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Tú defines el ID, no la BD
        [Column("account_number")]
        public int AccountNumber { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("account_type")]
        public string AccountType { get; set; } = string.Empty;

        [Column("initial_balance", TypeName = "decimal(10, 2)")]
        public decimal InitialBalance { get; set; } = 0.00m;

        [Column("status")]
        public bool Status { get; set; } = true;

        // Clave foránea
        [Column("customer_id")]
        public int CustomerId { get; set; }

        // Navegación hacia Cliente
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        // Relación: Una cuenta tiene muchas transacciones
        public virtual ICollection<TransactionBank> Transactions { get; set; } = new List<TransactionBank>();
    }
}
