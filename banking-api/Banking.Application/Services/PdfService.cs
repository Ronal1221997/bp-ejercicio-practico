using Banking.Application.DTOs.ReportDtos;
using Banking.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Banking.Application.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Configuración requerida por QuestPDF (Licencia comunitaria)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateStatementPdf(IEnumerable<ReporteEstadoCuentaDto> datos)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Usamos Landscape (Horizontal) porque la tabla tiene muchas columnas
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Reporte de Movimientos").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy}");
                        });
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        // Definir columnas (8 columnas según tu imagen)
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60); // Fecha
                            columns.RelativeColumn();   // Cliente
                            columns.ConstantColumn(70); // Cuenta
                            columns.ConstantColumn(60); // Tipo
                            columns.ConstantColumn(60); // Saldo Ini
                            columns.ConstantColumn(40); // Estado
                            columns.ConstantColumn(60); // Movimiento
                            columns.ConstantColumn(60); // Saldo Disp
                        });

                        // Encabezados
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Fecha");
                            header.Cell().Element(CellStyle).Text("Cliente");
                            header.Cell().Element(CellStyle).Text("Numero Cuenta");
                            header.Cell().Element(CellStyle).Text("Tipo");
                            header.Cell().Element(CellStyle).Text("Saldo Inicial");
                            header.Cell().Element(CellStyle).Text("Estado");
                            header.Cell().Element(CellStyle).Text("Movimiento");
                            header.Cell().Element(CellStyle).Text("Saldo Disp.");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).DefaultTextStyle(style => style.SemiBold());
                            }
                        });

                        // Filas de datos
                        foreach (var item in datos)
                        {
                            table.Cell().Element(DataStyle).Text(item.Fecha);
                            table.Cell().Element(DataStyle).Text(item.Cliente);
                            table.Cell().Element(DataStyle).Text(item.NumeroCuenta);
                            table.Cell().Element(DataStyle).Text(item.Tipo);
                            table.Cell().Element(DataStyle).Text(item.SaldoInicial.ToString("N2")); // N2 para 2 decimales
                            table.Cell().Element(DataStyle).Text(item.Estado.ToString());

                            // Color rojo si es negativo
                            table.Cell().Element(DataStyle).Text(item.Movimiento) // Ya viene formateado con el signo
                                .FontColor(item.Movimiento.Contains("-") ? Colors.Red.Medium : Colors.Black);

                            table.Cell().Element(DataStyle).Text(item.SaldoDisponible.ToString("N2"));

                            static IContainer DataStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(2).DefaultTextStyle(style => style.FontSize(10));
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }
    }
}
