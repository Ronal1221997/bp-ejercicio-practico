using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.DTOs.TransactionBankDtos
{
    public class TransactionBankUpdateDto
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
