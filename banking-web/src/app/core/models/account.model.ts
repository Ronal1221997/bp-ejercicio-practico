export interface Account {
  accountNumber: number; // El API lo usa como ID y número
  accountType: string;
  initialBalance: number;
  status: boolean;
  customerName?: string; // Viene en la respuesta
  customerId?: number;   // Necesario para el request
}