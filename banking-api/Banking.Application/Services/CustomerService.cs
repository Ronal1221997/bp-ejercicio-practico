using AutoMapper;
using Banking.Application.DTOs;
using Banking.Application.DTOs.CustomerDtos;
using Banking.Application.Interfaces;
using Banking.Application.Wrappers;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;

namespace Banking.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPersonRepository _personRepository; // Necesitamos verificar si la persona existe
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IPersonRepository personRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<CustomerResponseDto>> GetAllPagedAsync(PaginationFilter filter)
        {
            var (entities, totalCount) = await _customerRepository.GetAllPagedAsync(filter.PageNumber, filter.PageSize);
            var dtos = _mapper.Map<IEnumerable<CustomerResponseDto>>(entities);
            return new PagedResponse<CustomerResponseDto>(dtos, filter.PageNumber, filter.PageSize, totalCount);
        }

        public async Task<CustomerResponseDto> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) throw new KeyNotFoundException($"Cliente con ID {id} no encontrado.");

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        public async Task<CustomerResponseDto> CreateAsync(CustomerRequestDto request)
        {
            // 1. Validar que la persona existe
            var existingPerson = await _personRepository.GetByIdAsync(request.PersonId);
            if (existingPerson == null)
                throw new KeyNotFoundException($"No se puede crear cliente. La Persona con ID {request.PersonId} no existe.");

            // 2. Mapear y guardar
            var customer = _mapper.Map<Customer>(request);
            await _customerRepository.AddAsync(customer);

            // 3. Recargar el objeto con la Persona para devolver el DTO completo
            // (Opcional: asignar existingPerson manualmente para evitar ir a la BD de nuevo)
            customer.Person = existingPerson;

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        public async Task UpdateAsync(int id, CustomerRequestDto request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) throw new KeyNotFoundException($"Cliente con ID {id} no encontrado.");

            // Nota: Generalmente no permitimos cambiar el PersonId de un cliente, 
            // pero mapeamos el resto de campos (Password, Status)
            _mapper.Map(request, customer);

            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) throw new KeyNotFoundException($"Cliente con ID {id} no encontrado.");

            await _customerRepository.DeleteAsync(customer);
        }
    }
}
