export interface Transaction {
  transactionId?: number;
  date?: string;
  transactionType: string; // 'Deposito' | 'Retiro'
  amount: number;
  newBalance?: number;
  accountNumber: number;
}