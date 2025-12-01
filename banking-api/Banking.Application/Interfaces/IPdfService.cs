using Banking.Application.DTOs.ReportDtos;

namespace Banking.Application.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateStatementPdf(IEnumerable<ReporteEstadoCuentaDto> datos);
    }
}
