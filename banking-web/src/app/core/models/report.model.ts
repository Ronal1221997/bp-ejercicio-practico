export interface ReportItem {
  Fecha: string;
  Cliente: string;
  "Numero Cuenta": string;
  Tipo: string;
  "Saldo Inicial": number;
  Estado: boolean;
  Movimiento: string;
  "Saldo Disponible": number;
}

export interface ReportResponse {
  reporte: ReportItem[];
  archivoPdf?: string; // Cadena Base64
}