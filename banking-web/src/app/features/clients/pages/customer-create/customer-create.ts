import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { BankApi } from '../../../../core/services/bank-api';
import { CustomerRequest } from '../../../../core/models/customer.model';
import { Customer } from '../../../../core/models/customer.model';

@Component({
  selector: 'app-customer-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './customer-create.html',
  styleUrl: './customer-create.scss' // Opcional si creas estilos
})
export class CustomerCreate implements OnInit {
  
  private fb = inject(FormBuilder);
  private bankService = inject(BankApi);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  personId!: number;
  personName: string = ''; // Para mostrar a quién le creamos la cuenta

  // Variables nuevas para controlar el estado
  isEditMode = false;
  existingCustomerId?: number;

  form: FormGroup = this.fb.group({
    password: ['', [Validators.required, Validators.minLength(4)]],
    status: [true]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id) {
      this.personId = Number(id);
      
      // 1. Obtenemos nombre de la persona
      this.bankService.getClientById(this.personId).subscribe(p => {
        this.personName = p.name;
      });

      // 2. VERIFICAMOS SI YA ES CLIENTE
      this.checkIfCustomerExists();
    }
  }

  checkIfCustomerExists() {
    // Pedimos la lista de clientes existentes
    this.bankService.getCustomers().subscribe(customers => {
      // Buscamos si alguno coincide con el personId actual
      const found = customers.find(c => c.personId === this.personId);
      
      if (found) {
        this.isEditMode = true;
        this.existingCustomerId = found.customerId;
        
        // Rellenamos el estado actual
        this.form.patchValue({
          status: found.status,
          password: '' // La contraseña la dejamos vacía para que ingrese una nueva
        });
      }
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = {
      personId: this.personId,
      password: this.form.value.password,
      status: this.form.value.status
    };

    if (this.isEditMode && this.existingCustomerId) {
      // MODO ACTUALIZAR
      this.bankService.updateCustomer(this.existingCustomerId, request).subscribe({
        next: () => {
          alert('Credenciales actualizadas correctamente');
          this.router.navigate(['/clientes']);
        },
        error: (e) => console.error(e)
      });
    } else {
      // MODO CREAR
      this.bankService.createCustomer(request).subscribe({
        next: () => {
          alert('Cliente creado con éxito');
          this.router.navigate(['/clientes']);
        },
        error: (e) => console.error(e)
      });
    }
  }

}