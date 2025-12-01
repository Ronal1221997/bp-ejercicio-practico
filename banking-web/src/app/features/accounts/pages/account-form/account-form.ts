import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { BankApi } from '../../../../core/services/bank-api';
import { Customer } from '../../../../core/models/customer.model';
import { Account } from '../../../../core/models/account.model';
import { Observable } from 'rxjs'; 

@Component({
  selector: 'app-account-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './account-form.html',
  styleUrl: './account-form.scss'
})
export class AccountForm implements OnInit {
  
  private fb = inject(FormBuilder);
  private bankService = inject(BankApi);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  customers$: Observable<Customer[]> = this.bankService.getCustomers();
  
  accountNumber?: number;

  form: FormGroup = this.fb.group({
    accountNumber: ['', [Validators.required]],
    accountType: ['Ahorro', [Validators.required]],
    initialBalance: [0, [Validators.required, Validators.min(0)]],
    customerId: ['', [Validators.required]],
    status: [true]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id) {
      // --- MODO EDICIÓN ---
      this.accountNumber = Number(id);
      
      // Bloqueos de edición
      this.form.get('accountNumber')?.disable();
      this.form.get('customerId')?.disable(); 
      this.form.get('accountType')?.disable();    
      this.form.get('initialBalance')?.disable(); 
      
      this.bankService.getAccountById(this.accountNumber).subscribe(acc => {
        if (acc) this.form.patchValue(acc);
      });

    } else {
      // --- MODO CREACIÓN (NUEVO) ---
      
      // 1. Generar número aleatorio de 6 dígitos (100000 - 999999)
      const randomId = Math.floor(100000 + Math.random() * 900000);
      
      // 2. Asignarlo al formulario
      this.form.patchValue({
        accountNumber: randomId
      });

      // 3. Bloquear el campo para que el usuario no lo edite
      this.form.get('accountNumber')?.disable();
    }
  }

  save() {
     if (this.form.invalid) return;
     
     // getRawValue() es CLAVE aquí: captura el valor de accountNumber
     // aunque esté deshabilitado (disable).
     const accountData = this.form.getRawValue() as Account;
     
     if (this.accountNumber) {
       this.bankService.updateAccount(this.accountNumber, accountData).subscribe(() => {
         this.router.navigate(['/cuentas']);
       });
     } else {
       this.bankService.addAccount(accountData).subscribe(() => {
         this.router.navigate(['/cuentas']);
       });
     }
  }
}