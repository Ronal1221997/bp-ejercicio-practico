import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { BankApi } from '../../../../core/services/bank-api';
import { Account } from '../../../../core/models/account.model';
import { Transaction } from '../../../../core/models/transaction.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './transaction-form.html',
  styleUrl: './transaction-form.scss'
})
export class TransactionForm implements OnInit {
  
  private fb = inject(FormBuilder);
  private bankService = inject(BankApi);
  private router = inject(Router);

  // Cargamos cuentas para el dropdown
  accounts$: Observable<Account[]> = this.bankService.getAccounts();

  form: FormGroup = this.fb.group({
    accountNumber: ['', [Validators.required]],
    transactionType: ['Deposito', [Validators.required]],
    amount: ['', [Validators.required, Validators.min(0.01)]]
  });

  ngOnInit() {
    // No necesitamos lógica de carga extra gracias al AsyncPipe en el HTML
  }

  save() {
    if (this.form.invalid) return;

    const txData = this.form.value as Transaction;

    this.bankService.createTransaction(txData).subscribe({
      next: () => {
        this.router.navigate(['/movimientos']);
      },
      error: (err) => alert('Error al procesar la transacción')
    });
  }
}