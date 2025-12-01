using Banking.Application.DTOs;
using Banking.Application.DTOs.CustomerDtos;
using Banking.Application.Interfaces;
using Banking.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace banking_api.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("readall")]
        public async Task<ActionResult<PagedResponse<CustomerResponseDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Validación defensiva: Evitar consultas inválidas a la BD
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "PageNumber y PageSize deben ser mayores a 0." });
            }

            var filter = new PaginationFilter(pageNumber, pageSize);
            var response = await _customerService.GetAllPagedAsync(filter);

            return Ok(response);
        }

        [HttpGet("readbycustomerid/{id}")]
        public async Task<ActionResult<CustomerResponseDto>> GetById(int id)
        {
            // El servicio busca el cliente. 
            // Si no existe, lanza KeyNotFoundException y el Middleware retorna automáticamente el 404.
            var customer = await _customerService.GetByIdAsync(id);

            return Ok(customer);
        }

        [HttpPost("create")]
        public async Task<ActionResult<CustomerResponseDto>> Create(CustomerRequestDto request)
        {
            // El servicio se encarga de la lógica.
            // Si la Persona asociada no existe, lanza KeyNotFoundException -> Middleware -> 404 Not Found.
            var createdCustomer = await _customerService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdCustomer.CustomerId }, createdCustomer);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<bool>> Update(int id, CustomerRequestDto request)
        {

            // El servicio realiza la actualización.
            // Si el ID no existe, la excepción sube al Middleware y este retorna el 404.
            await _customerService.UpdateAsync(id, request);

            // Retornamos 200 OK con true, para cumplir con la firma ActionResult<bool>
            return Ok(true);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            // El servicio ejecuta la eliminación lógica o física.
            // Si el ID no existe, la excepción sube al Middleware Global (404).
            await _customerService.DeleteAsync(id);

            // Retornamos 200 OK con true, cumpliendo con la firma ActionResult<bool>
            return Ok(true);
        }
    }
}
