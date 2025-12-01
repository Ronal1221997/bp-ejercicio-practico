using Banking.Application.DTOs;
using Banking.Application.DTOs.AccountDtos;
using Banking.Application.Interfaces;
using Banking.Application.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace banking_api.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpGet("readall")]
        public async Task<ActionResult<PagedResponse<AccountResponseDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Validación defensiva
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("PageNumber y PageSize deben ser mayores a 0.");
            }

            var filter = new PaginationFilter(pageNumber, pageSize);
            var response = await _accountService.GetAllPagedAsync(filter);
            return Ok(response);
        }

        [HttpGet("readbyaccountid/{id}")]
        public async Task<ActionResult<AccountResponseDto>> GetById(int id)
        {

            var account = await _accountService.GetByIdAsync(id);
            return Ok(account);
        }

        [HttpPost("create")]
        public async Task<ActionResult<AccountResponseDto>> Create(AccountRequestDto request)
        {
            try
            {
                var createdAccount = await _accountService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = createdAccount.AccountNumber }, createdAccount);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // Captura "Cuenta ya existe"
            {
                return Conflict(new { message = ex.Message }); // 409 Conflict es correcto para duplicados
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<bool>> Update(int id, AccountRequestDto request)
        {
            // Validación de consistencia
            if (id != request.AccountNumber)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el cuerpo de la petición." });
            }

            // El servicio ejecuta la actualización. 
            // Si falla (ej. no existe el ID), lanza excepción y el Middleware devuelve el error (404/400/500).
            await _accountService.UpdateAsync(id, request);

            // Retorna HTTP 200 con el valor "true" en el cuerpo
            return Ok(true);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            // El servicio intenta eliminar. 
            // Si el ID no existe, lanza KeyNotFoundException y el Middleware se encarga del 404.
            await _accountService.DeleteAsync(id);

            // Retornamos 200 OK con true, siguiendo tu requisito de devolver booleano
            return Ok(true);
        }
    }
}
