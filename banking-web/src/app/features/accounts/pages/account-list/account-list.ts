import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BankApi } from '../../../../core/services/bank-api';
import { Account } from '../../../../core/models/account.model';
// IMPORTANTE: Agregar 'of' y 'tap'
import { Observable, of, tap } from 'rxjs';

@Component({
  selector: 'app-account-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './account-list.html',
  styleUrl: './account-list.scss'
})
export class AccountList {
  
  private bankService = inject(BankApi);

  // Variable para guardar la copia original de los datos
  allAccounts: Account[] = [];

  // Usamos .pipe(tap(...)) para capturar los datos apenas lleguen del API
  accounts$: Observable<Account[]> = this.bankService.getAccounts().pipe(
    tap(data => this.allAccounts = data)
  );

  deleteAccount(acc: Account) {
    if (confirm(`¿Eliminar la cuenta ${acc.accountNumber}?`)) {
      this.bankService.deleteAccount(acc.accountNumber).subscribe(() => {
        // Recargamos y volvemos a actualizar la copia
        this.accounts$ = this.bankService.getAccounts().pipe(
          tap(data => this.allAccounts = data)
        );
      });
    }
  }

  // --- LÓGICA DEL BUSCADOR ---
  onSearch(event: Event) {
    const term = (event.target as HTMLInputElement).value.toLowerCase();

    if (!term) {
      // Si borran, restauramos la lista original usando 'of'
      this.accounts$ = of(this.allAccounts);
    } else {
      // Filtramos
      const filtered = this.allAccounts.filter(acc => 
        // Convertimos el número a string para buscar
        acc.accountNumber.toString().includes(term) || 
        // Buscamos por nombre de cliente (si existe)
        acc.customerName?.toLowerCase().includes(term)
      );
      this.accounts$ = of(filtered);
    }
  }
}