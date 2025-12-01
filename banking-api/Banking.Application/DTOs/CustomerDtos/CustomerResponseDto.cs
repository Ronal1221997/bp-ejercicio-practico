using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.DTOs.CustomerDtos
{
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool Status { get; set; }

        public int PersonId { get; set; }
        // Incluimos el nombre para no tener que hacer dos peticiones en el frontend
        public string PersonName { get; set; } = string.Empty;
    }
}
