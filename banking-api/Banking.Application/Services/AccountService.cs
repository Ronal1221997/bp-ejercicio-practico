using AutoMapper;
using Banking.Application.DTOs;
using Banking.Application.DTOs.AccountDtos;
using Banking.Application.Interfaces;
using Banking.Application.Wrappers;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository; // Para validar cliente
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, ICustomerRepository customerRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<AccountResponseDto>> GetAllPagedAsync(PaginationFilter filter)
        {
            var (entities, totalCount) = await _accountRepository.GetAllPagedAsync(filter.PageNumber, filter.PageSize);
            var dtos = _mapper.Map<IEnumerable<AccountResponseDto>>(entities);
            return new PagedResponse<AccountResponseDto>(dtos, filter.PageNumber, filter.PageSize, totalCount);
        }

        public async Task<AccountResponseDto> GetByIdAsync(int accountNumber)
        {
            var account = await _accountRepository.GetByIdAsync(accountNumber);
            if (account == null) throw new KeyNotFoundException($"Cuenta número {accountNumber} no encontrada.");

            return _mapper.Map<AccountResponseDto>(account);
        }

        public async Task<AccountResponseDto> CreateAsync(AccountRequestDto request)
        {
            // 1. Validar que el cliente exista
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new KeyNotFoundException($"Cliente con ID {request.CustomerId} no encontrado.");

            // 2. Validar que el número de cuenta NO exista ya
            var existingAccount = await _accountRepository.GetByIdAsync(request.AccountNumber);
            if (existingAccount != null)
                throw new InvalidOperationException($"El número de cuenta {request.AccountNumber} ya existe.");

            // 3. Crear
            var account = _mapper.Map<Account>(request);
            await _accountRepository.AddAsync(account);

            // Rellenar datos de navegación para el response
            account.Customer = customer;

            return _mapper.Map<AccountResponseDto>(account);
        }

        public async Task UpdateAsync(int accountNumber, AccountRequestDto request)
        {
            var account = await _accountRepository.GetByIdAsync(accountNumber);
            if (account == null) throw new KeyNotFoundException($"Cuenta número {accountNumber} no encontrada.");

            // Mapeamos los cambios (Tipo, Saldo, Estado)
            _mapper.Map(request, account);

            // Ojo: Generalmente no permitimos cambiar el AccountNumber (PK) ni el CustomerId fácilmente
            // pero el Mapper lo intentará si están en el DTO.
            // Aseguramos que el ID se mantenga por integridad si fuera necesario:
            account.AccountNumber = accountNumber;

            await _accountRepository.UpdateAsync(account);
        }

        public async Task DeleteAsync(int accountNumber)
        {
            var account = await _accountRepository.GetByIdAsync(accountNumber);
            if (account == null) throw new KeyNotFoundException($"Cuenta número {accountNumber} no encontrada.");

            await _accountRepository.DeleteAsync(account);
        }
    }
}
