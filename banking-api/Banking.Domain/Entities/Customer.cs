using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Domain.Entities
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Column("status")]
        public bool Status { get; set; } = true;

        // Clave foránea hacia Person
        [Column("person_id")]
        public int PersonId { get; set; }

        // Propiedad de navegación hacia el Padre
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; } = null!;

        // Relación: Un cliente tiene muchas cuentas
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
