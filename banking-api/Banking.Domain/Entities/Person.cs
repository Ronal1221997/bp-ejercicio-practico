using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Domain.Entities
{
    [Table("Person")]
    public class Person
    {
        [Key]
        [Column("person_id")]
        public int PersonId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("gender")]
        public string? Gender { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("identification")]
        public string Identification { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("address")]
        public string? Address { get; set; }

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        // Navegación (Relación 1 a 1 lógica con Customer)
        public virtual Customer? Customer { get; set; }
    }
}
