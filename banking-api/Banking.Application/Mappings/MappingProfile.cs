using AutoMapper;
using Banking.Application.DTOs;
using Banking.Application.DTOs.AccountDtos;
using Banking.Application.DTOs.CustomerDtos;
using Banking.Application.DTOs.PersonDtos;
using Banking.Domain.Entities;

namespace Banking.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Configuración: Mapear de DTO a Entidad
            CreateMap<TransactionBankRequestDto, TransactionBank>()
                .ReverseMap(); // Esto permite mapear en ambas direcciones si fuera necesario

            // Configuración: Mapear de Entidad a DTO de Respuesta
            CreateMap<TransactionBank, TransactionBankResponseDto>()
                .ForMember(dest => dest.NewBalance, opt => opt.MapFrom(src => src.Balance));


            // Dentro del constructor MappingProfile()
            CreateMap<PersonRequestDto, Person>();
            CreateMap<Person, PersonResponseDto>();

            CreateMap<CustomerRequestDto, Customer>();

            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Person.Name));

            // Mapeo Cuenta
            CreateMap<AccountRequestDto, Account>();

            CreateMap<Account, AccountResponseDto>()
                // Aplanamos: Sacamos el nombre de la persona a través del cliente
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Person.Name));
        }
    }
}
