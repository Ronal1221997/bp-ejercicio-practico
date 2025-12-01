using System.Text.Json.Serialization;

namespace Banking.Application.DTOs.ReportDtos
{
    public class ReporteEstadoCuentaDto
    {
        [JsonPropertyName("Fecha")]
        public string Fecha { get; set; } = string.Empty;

        [JsonPropertyName("Cliente")]
        public string Cliente { get; set; } = string.Empty;

        [JsonPropertyName("Numero Cuenta")] // Clave con espacio
        public string NumeroCuenta { get; set; } = string.Empty;

        [JsonPropertyName("Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [JsonPropertyName("Saldo Inicial")] // Clave con espacio
        public decimal SaldoInicial { get; set; }

        [JsonPropertyName("Estado")]
        public bool Estado { get; set; }

        [JsonPropertyName("Movimiento")]
        public string Movimiento { get; set; } = string.Empty;

        [JsonPropertyName("Saldo Disponible")] // Clave con espacio
        public decimal SaldoDisponible { get; set; }
    }
}
