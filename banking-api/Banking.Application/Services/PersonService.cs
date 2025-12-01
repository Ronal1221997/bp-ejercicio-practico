using AutoMapper;
using Banking.Application.DTOs;
using Banking.Application.DTOs.PersonDtos;
using Banking.Application.Interfaces;
using Banking.Application.Wrappers;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;

namespace Banking.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public PersonService(IPersonRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<PersonResponseDto>> GetAllPagedAsync(PaginationFilter filter)
        {
            var (entities, totalCount) = await _personRepository.GetAllPagedAsync(filter.PageNumber, filter.PageSize);
            var dtos = _mapper.Map<IEnumerable<PersonResponseDto>>(entities);
            return new PagedResponse<PersonResponseDto>(dtos, filter.PageNumber, filter.PageSize, totalCount);
        }

        public async Task<PersonResponseDto> GetByIdAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null) throw new KeyNotFoundException($"Persona con ID {id} no encontrada.");

            return _mapper.Map<PersonResponseDto>(person);
        }

        public async Task<PersonResponseDto> CreateAsync(PersonRequestDto request)
        {
            var person = _mapper.Map<Person>(request);
            await _personRepository.AddAsync(person);
            return _mapper.Map<PersonResponseDto>(person);
        }

        public async Task UpdateAsync(int id, PersonRequestDto request)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null) throw new KeyNotFoundException($"Persona con ID {id} no encontrada.");

            // AutoMapper actualiza las propiedades del objeto existente con las del request
            _mapper.Map(request, person);

            await _personRepository.UpdateAsync(person);
        }

        public async Task DeleteAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null) throw new KeyNotFoundException($"Persona con ID {id} no encontrada.");

            await _personRepository.DeleteAsync(person);
        }
    }
}
