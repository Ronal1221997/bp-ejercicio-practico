import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BankApi } from '../../../../core/services/bank-api';
import { Transaction } from '../../../../core/models/transaction.model';
// 1. IMPORTANTE: Agregar 'of' y 'tap'
import { Observable, of, tap } from 'rxjs';

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './transaction-list.html',
  styleUrl: './transaction-list.scss'
})
export class TransactionList {
  
  private bankService = inject(BankApi);

  // 2. Variable para guardar la copia original (Backup)
  allTransactions: Transaction[] = [];

  // 3. Modificamos el observable para guardar la copia apenas lleguen los datos
  transactions$: Observable<Transaction[]> = this.bankService.getTransactions().pipe(
    tap(data => this.allTransactions = data)
  );

  // 4. Función del Buscador
  onSearch(event: Event) {
    const term = (event.target as HTMLInputElement).value.toLowerCase();

    if (!term) {
      // Si borran, restauramos la lista completa
      this.transactions$ = of(this.allTransactions);
    } else {
      // Filtramos por Número de Cuenta o Tipo (Deposito/Retiro)
      const filtered = this.allTransactions.filter(tx => 
        tx.accountNumber.toString().includes(term) || 
        tx.transactionType.toLowerCase().includes(term)
      );
      this.transactions$ = of(filtered);
    }
  }
}