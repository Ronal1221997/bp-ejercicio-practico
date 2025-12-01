using System.ComponentModel.DataAnnotations;

namespace Banking.Application.DTOs.CustomerDtos
{
    public class CustomerRequestDto
    {
        [Required]
        public string Password { get; set; } = string.Empty; // En producción, esto se hashea

        public bool Status { get; set; } = true;

        [Required]
        public int PersonId { get; set; } // El ID de la persona existente
    }
}
