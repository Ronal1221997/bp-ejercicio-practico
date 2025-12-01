using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.DTOs.PersonDtos
{
    public class PersonRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public int Age { get; set; }

        [Required]
        public string Identification { get; set; } = string.Empty;

        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
