import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BankApi } from '../../../../core/services/bank-api';
import { Client } from '../../../../core/models/client.model';
import { RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
// CORRECCIÓN AQUÍ: Agregamos ', of' a la importación
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [CommonModule, RouterLink, AsyncPipe], // Asegúrate de tener AsyncPipe aquí también
  templateUrl: './client-list.html',
  styleUrl: './client-list.scss'
})
export class ClientList implements OnInit {

  // 1. Inyectamos el servicio
  private bankService = inject(BankApi);

  // 2. Variable para guardar los datos que lleguen
  clients: Client[] = [];

  clients$: Observable<Client[]> = this.bankService.getClients();

  allClients: Client[] = [];
  
  constructor() {
     // Cargamos los datos iniciales y guardamos una copia para filtrar
     this.clients$.subscribe(data => this.allClients = data);
  }

  // 3. Al iniciar el componente, pedimos los datos
  ngOnInit() {
    this.loadClients();
  }

  loadClients() {
    this.bankService.getClients().subscribe({
      next: (data) => {
        console.log('Datos recibidos del API:', data);
        this.clients = data;
        // Actualizamos también la copia de respaldo por si acaso
        this.allClients = data; 
      },
      error: (err) => console.error('Error cargando clientes', err)
    });
  }

  deleteClient(client: Client) {
    if (confirm('¿Eliminar?')) {
      this.bankService.deleteClient(client.personId ?? 0).subscribe(() => {
        // TRUCO: Reasignamos el observable para que vuelva a pedir los datos al API
        this.clients$ = this.bankService.getClients();
        // También actualizamos la lista interna para que el buscador siga funcionando
        this.clients$.subscribe(data => this.allClients = data);
      });
    }
  }

  // Método para el buscador
  onSearch(event: Event) {
    const term = (event.target as HTMLInputElement).value.toLowerCase();

    // Opción Sencilla: Filtramos la copia y emitimos un nuevo valor
    if (!term) {
      // Si borran el texto, volvemos a pedir los datos originales al servicio
      this.clients$ = this.bankService.getClients();
    } else {
      // Filtramos sobre la copia que guardamos (allClients)
      const filtered = this.allClients.filter(c => 
          c.name.toLowerCase().includes(term) || 
          c.identification.includes(term)
      );
      // Convertimos el array filtrado en un Observable para que el HTML lo lea
      // AHORA SÍ FUNCIONARÁ PORQUE IMPORTAMOS 'of'
      this.clients$ = of(filtered);
    }
  }
}