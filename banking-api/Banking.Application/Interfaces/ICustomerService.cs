using Banking.Application.DTOs;
using Banking.Application.DTOs.CustomerDtos;
using Banking.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResponse<CustomerResponseDto>> GetAllPagedAsync(PaginationFilter filter);
        Task<CustomerResponseDto> GetByIdAsync(int id);
        Task<CustomerResponseDto> CreateAsync(CustomerRequestDto request);
        Task UpdateAsync(int id, CustomerRequestDto request);
        Task DeleteAsync(int id);
    }
}
