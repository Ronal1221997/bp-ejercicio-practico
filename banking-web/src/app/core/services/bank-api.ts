import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs'; // Importamos map
import { environment } from '../../../environments/environment';
import { Client } from '../models/client.model';
import { Customer, CustomerRequest } from '../models/customer.model';
import { Account } from '../models/account.model';
import { Transaction } from '../models/transaction.model';
import {  HttpParams } from '@angular/common/http';
import { ReportResponse } from '../models/report.model';

@Injectable({
  providedIn: 'root'
})
export class BankApi {
  
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // --- PERSONAS (Tu vista de Clientes) ---
  getClients(): Observable<Client[]> {
    // El API devuelve { data: [...] }, extraemos 'data'
    return this.http.get<any>(`${this.apiUrl}/personas/readall`)
      .pipe(map(response => response.data));
  }

  getClientById(id: number): Observable<Client> {
  return this.http.get<any>(`${this.apiUrl}/personas/readbypersonid/${id}`);
}

  addClient(person: Client): Observable<any> {
    return this.http.post(`${this.apiUrl}/personas/create`, person);
  }

  updateClient(id: number, person: Client): Observable<any> {
  return this.http.put(`${this.apiUrl}/personas/update/${id}`, person);
}

  deleteClient(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/personas/delete/${id}`);
  }

 // --- CLIENTES (Para llenar el select de Cuentas) ---
  getCustomers(): Observable<Customer[]> {
    // Definimos parámetros para traer muchos registros (ej. 1000) y evitar la paginación por defecto de 10
    const params = new HttpParams()
      .set('pageNumber', 1)
      .set('pageSize', 1000); 

    return this.http.get<any>(`${this.apiUrl}/clientes/readall`, { params })
      .pipe(map(response => response.data));
  }

  // CREAR CLIENTE (Asignar usuario y contraseña a una persona)
  createCustomer(customer: CustomerRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/clientes/create`, customer);
  }

  // ACTUALIZAR CLIENTE (Para cambiar contraseña o estado)
  updateCustomer(id: number, customer: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/clientes/update/${id}`, customer);
  }

  // --- CUENTAS ---
  getAccounts(): Observable<Account[]> {
    return this.http.get<any>(`${this.apiUrl}/cuentas/readall`)
      .pipe(map(response => response.data));
  }

  // LEER UNA POR ID (Viene directo, SIN map)
  getAccountById(id: number): Observable<Account> {
    return this.http.get<Account>(`${this.apiUrl}/cuentas/readbyaccountid/${id}`);
  }

  // CREAR
  addAccount(account: Account): Observable<any> {
    return this.http.post(`${this.apiUrl}/cuentas/create`, account);
  }

  // ACTUALIZAR
  updateAccount(id: number, account: Account): Observable<any> {
    return this.http.put(`${this.apiUrl}/cuentas/update/${id}`, account);
  }

  // ELIMINAR
  deleteAccount(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/cuentas/delete/${id}`);
  }

  // --- MOVIMIENTOS ---

  // 1. Obtener TODOS los movimientos (Historial Global)
  getTransactions(): Observable<Transaction[]> {
    const params = new HttpParams()
      .set('pageNumber', 1)
      .set('pageSize', 1000); // Traemos bastantes para ver el historial

    return this.http.get<any>(`${this.apiUrl}/movimientos/readall`, { params })
      .pipe(map(response => response.data));
  }

  // 2. Crear Movimiento (Depósito/Retiro)
  createTransaction(tx: Transaction): Observable<any> {
    // Ajustamos al DTO: TransactionBankRequestDto
    const payload = {
      accountNumber: Number(tx.accountNumber),
      amount: Number(tx.amount),
      transactionType: tx.transactionType
    };
    return this.http.post(`${this.apiUrl}/movimientos/create`, payload);
  }

  // --- REPORTES ---
  getReports(startDate: string, endDate: string, clientId: number): Observable<ReportResponse> {
    const params = new HttpParams()
      .set('fechaInicio', startDate)
      .set('fechaFin', endDate)
      .set('clienteId', clientId)
      .set('pageNumber', 1)
      .set('pageSize', 1000);

    // El endpoint devuelve { reporte: [...], archivoPdf: "..." }
    return this.http.get<ReportResponse>(`${this.apiUrl}/reportes/movimientosporfechas`, { params });
  }



}