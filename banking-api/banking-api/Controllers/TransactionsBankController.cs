using Banking.Application.DTOs;
using Banking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Banking.Application.Wrappers;
using Banking.Application.DTOs.TransactionBankDtos;

namespace banking_api.Controllers
{
    [ApiController]
    [Route("api/movimientos")]
    public class TransactionsBankController : ControllerBase
    {
        private readonly ITransactionBankService _transactionService;

        // Inyección del Servicio (NO del repositorio, ni del DbContext)
        public TransactionsBankController(ITransactionBankService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<TransactionBankResponseDto>> Create([FromBody] TransactionBankRequestDto request)
        {
            // 1. La validación del modelo es automática gracias a [ApiController].

            // 2. El servicio ejecuta la transacción.
            //    - Si hay "Saldo Insuficiente", lanza InvalidOperationException -> Middleware -> 400 BadRequest.
            //    - Si falla la BD, lanza Exception -> Middleware -> 500 Internal Error.
            var result = await _transactionService.CreateTransactionAsync(request);

            // 3. Retornar 201 Created SIN dependencia de GetHistory
            // Usamos StatusCode explícito. Esto devuelve el objeto creado en el body
            // pero no intenta construir un header 'Location'.
            return StatusCode(StatusCodes.Status201Created, result);
        }



        [HttpGet("readall")]
        public async Task<ActionResult<PagedResponse<TransactionBankResponseDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Validación de Paginación
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "PageNumber y PageSize deben ser mayores a 0." });
            }

            var filter = new PaginationFilter(pageNumber, pageSize);
            var response = await _transactionService.GetAllTransactionsPagedAsync(filter);

            return Ok(response);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] TransactionBankUpdateDto request)
        {
            // Validación de ID
            if (id <= 0)
            {
                return BadRequest(new { message = "ID inválido." });
            }

            // Validación de consistencia (Opcional, si el DTO trae ID)
            // if (request.Id != id) return BadRequest(new { message = "El ID de la URL no coincide con el cuerpo." });

            // El Middleware maneja excepciones (KeyNotFound, etc.)
            await _transactionService.UpdateTransactionAsync(id, request);

            // Retorno explícito de true solicitado
            return Ok(true);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            // Validación de ID
            if (id <= 0)
            {
                return BadRequest(new { message = "ID inválido." });
            }

            // El Middleware maneja excepciones
            await _transactionService.DeleteTransactionAsync(id);

            // Retorno explícito de true solicitado
            return Ok(true);
        }


    }


}
