using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Banking.Application.DTOs.ReportDtos
{
    public class ReporteFinalDto
    {
        // Esta propiedad contendrá tu JSON con el formato exacto de la imagen
        [JsonPropertyName("reporte")]
        public IEnumerable<ReporteEstadoCuentaDto> ReporteJson { get; set; }

        // Esta propiedad contendrá el PDF en Base64
        [JsonPropertyName("archivoPdf")]
        public string ArchivoPdfBase64 { get; set; } = string.Empty;
    }
}
