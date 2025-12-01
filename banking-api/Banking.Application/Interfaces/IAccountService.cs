using Banking.Application.DTOs;
using Banking.Application.DTOs.AccountDtos;
using Banking.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.Interfaces
{
    public interface IAccountService
    {
        Task<PagedResponse<AccountResponseDto>> GetAllPagedAsync(PaginationFilter filter);
        Task<AccountResponseDto> GetByIdAsync(int accountNumber);
        Task<AccountResponseDto> CreateAsync(AccountRequestDto request);
        Task UpdateAsync(int accountNumber, AccountRequestDto request);
        Task DeleteAsync(int accountNumber);
    }
}
