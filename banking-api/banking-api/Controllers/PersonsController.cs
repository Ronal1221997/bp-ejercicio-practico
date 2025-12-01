using Banking.Application.DTOs;
using Banking.Application.DTOs.PersonDtos;
using Banking.Application.Interfaces;
using Banking.Application.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace banking_api.Controllers
{
    [ApiController]
    [Route("api/personas")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("readall")]
        public async Task<ActionResult<PagedResponse<PersonResponseDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Validación defensiva: Evitamos números negativos o cero antes de ir al servicio
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "PageNumber y PageSize deben ser mayores a 0." });
            }

            var filter = new PaginationFilter(pageNumber, pageSize);
            var response = await _personService.GetAllPagedAsync(filter);

            return Ok(response);
        }

        [HttpGet("readbypersonid/{id}")]
        public async Task<ActionResult<PersonResponseDto>> GetById(int id)
        {
            // El servicio busca la persona.
            // Si no existe, lanza KeyNotFoundException y el Middleware se encarga de retornar el 404.
            var person = await _personService.GetByIdAsync(id);

            return Ok(person);
        }

        [HttpPost("create")]
        public async Task<ActionResult<PersonResponseDto>> Create(PersonRequestDto request)
        {
            var createdPerson = await _personService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdPerson.PersonId }, createdPerson);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<bool>> Update(int id, PersonRequestDto request)
        {
            // Validación de consistencia
            // Asumimos que PersonRequestDto tiene una propiedad PersonId (o Id)
            // El servicio actualiza.
            // Si la persona no existe, la excepción sube al Middleware Global.
            await _personService.UpdateAsync(id, request);

            // Retornamos 200 OK con true
            return Ok(true);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            // El servicio intenta eliminar.
            // Si el ID no existe, lanza KeyNotFoundException y el Middleware Global devuelve el 404.
            await _personService.DeleteAsync(id);

            // Retornamos 200 OK con true, cumpliendo con la firma ActionResult<bool>
            return Ok(true);
        }

    }
}
