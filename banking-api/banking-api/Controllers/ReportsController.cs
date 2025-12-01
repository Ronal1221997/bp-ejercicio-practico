using Banking.Application.DTOs.ReportDtos;
using Banking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace banking_api.Controllers
{
    [ApiController]
    [Route("api/reportes")] // Mapea a /reportes (prefijo api es estándar en .NET, pero puedes quitarlo)
    public class ReportsController : ControllerBase
    {
        private readonly ITransactionBankService _transactionService;

        public ReportsController(ITransactionBankService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("movimientosporfechas")] // Opcional: [HttpGet("por-fechas")] si tienes otros Gets en este controller
        public async Task<ActionResult<ReporteFinalDto>> GetReporte(
    [FromQuery] DateTime fechaInicio,
    [FromQuery] DateTime fechaFin,
    [FromQuery] int clienteId)
        {
            // 1. Validación Defensiva: Rango de fechas coherente
            if (fechaFin < fechaInicio)
            {
                return BadRequest(new { message = "La fecha de fin no puede ser anterior a la fecha de inicio." });
            }

            

            // 3. Llamada limpia al servicio
            // Si el cliente no existe o no tiene cuentas, el servicio puede lanzar 
            // KeyNotFoundException, que será capturado por tu Middleware Global (404).
            var resultado = await _transactionService.GetReportePorFechasAsync(clienteId, fechaInicio, fechaFin);

            return Ok(resultado);
        }


    }
}
