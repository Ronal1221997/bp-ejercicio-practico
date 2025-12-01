using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.DTOs.AccountDtos
{
    public class AccountResponseDto
    {
        public int AccountNumber { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
        public bool Status { get; set; }
        public string CustomerName { get; set; } = string.Empty; // Útil para mostrar
    }
}
