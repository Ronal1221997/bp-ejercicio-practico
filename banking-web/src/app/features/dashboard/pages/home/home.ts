import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common'; // CurrencyPipe para el dinero
import { RouterLink } from '@angular/router';
import { BankApi } from '../../../../core/services/bank-api';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyPipe],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home implements OnInit {
  
  private bankService = inject(BankApi);

  // Variables para las estadísticas
  totalClients = 0;
  totalAccounts = 0;
  totalMoney = 0;

  ngOnInit() {
    // 1. Contar Clientes
    this.bankService.getClients().subscribe(list => {
      this.totalClients = list.length;
    });

    // 2. Contar Cuentas y Sumar el dinero total del banco
    this.bankService.getAccounts().subscribe(list => {
      this.totalAccounts = list.length;
      // Usamos reduce para sumar el saldo de todas las cuentas
      this.totalMoney = list.reduce((total, account) => total + account.initialBalance, 0);
    });
  }
}