import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { BankApi } from '../../../../core/services/bank-api';
import { Client } from '../../../../core/models/client.model';

@Component({
  selector: 'app-client-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './client-form.html',
  styleUrl: './client-form.scss'
})
export class ClientForm implements OnInit {
  
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private bankService = inject(BankApi);

  personId?: number; // Usamos personId en lugar de clientId

  // CORRECCIÓN: El formulario ahora coincide exactamente con el JSON del API
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    identification: ['', [Validators.required]],
    gender: ['Masculino', [Validators.required]], // Valor por defecto
    age: ['', [Validators.required, Validators.min(18)]],
    address: ['', [Validators.required]],
    phone: ['', [Validators.required]]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.personId = Number(id);
      // Cargar datos si es edición
      this.bankService.getClientById(this.personId).subscribe(client => {
        if (client) {
          // patchValue llena el formulario automáticamente si los nombres coinciden
          this.form.patchValue(client);
        }
      });
    }
  }

  save() {
    if (this.form.invalid) return;

    const personData = this.form.value as Client;

    if (this.personId) {
      // EDICIÓN
      this.bankService.updateClient(this.personId, personData).subscribe({
        next: () => this.router.navigate(['/clientes']),
        error: (e) => console.error(e)
      });
    } else {
      // CREACIÓN (Nuevo Cliente)
      this.bankService.addClient(personData).subscribe({
        next: () => this.router.navigate(['/clientes']),
        error: (e) => console.error(e)
      });
    }
  }
}