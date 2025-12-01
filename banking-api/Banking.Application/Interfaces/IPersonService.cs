using Banking.Application.DTOs;
using Banking.Application.DTOs.PersonDtos;
using Banking.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.Interfaces
{
    public interface IPersonService
    {
        Task<PagedResponse<PersonResponseDto>> GetAllPagedAsync(PaginationFilter filter);
        Task<PersonResponseDto> GetByIdAsync(int id);
        Task<PersonResponseDto> CreateAsync(PersonRequestDto request);
        Task UpdateAsync(int id, PersonRequestDto request);
        Task DeleteAsync(int id);
    }
}
