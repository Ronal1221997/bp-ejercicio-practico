using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.DTOs.AccountDtos
{
    public class AccountRequestDto
    {
        [Required]
        public int AccountNumber { get; set; } // El usuario o frontend decide el número

        [Required]
        public string AccountType { get; set; } = string.Empty; // Ahorros / Corriente

        public decimal InitialBalance { get; set; } = 0;

        public bool Status { get; set; } = true;

        [Required]
        public int CustomerId { get; set; } // ¿De quién es la cuenta?
    }
}
