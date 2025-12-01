import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { BankApi } from './bank-api';
import { environment } from '../../../environments/environment';
import { Client } from '../models/client.model';
import { Account } from '../models/account.model';
import { Transaction } from '../models/transaction.model';

describe('BankApi Service', () => {
  let service: BankApi;
  let httpMock: HttpTestingController;
  const apiUrl = environment.apiUrl; // Usamos la URL real de tu entorno

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        BankApi,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(BankApi);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Verifica que no queden peticiones pendientes
  });

  // ==========================================
  // 1. PRUEBAS DE PERSONAS (CLIENTES UI)
  // ==========================================
  describe('Gestión de Personas', () => {
    
    it('getClients: debe extraer el array .data de la respuesta paginada', () => {
      const mockResponse = {
        pageNumber: 1,
        totalRecords: 1,
        data: [
          { personId: 1, name: 'Jose', identification: '123' }
        ]
      };

      service.getClients().subscribe(clients => {
        expect(clients.length).toBe(1);
        expect(clients[0].name).toBe('Jose');
      });

      const req = httpMock.expectOne(`${apiUrl}/personas/readall`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    it('addClient: debe enviar una petición POST', () => {
      const newPerson = { name: 'Maria', identification: '999' } as Client;

      service.addClient(newPerson).subscribe(resp => {
        expect(resp).toBeTruthy();
      });

      const req = httpMock.expectOne(`${apiUrl}/personas/create`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(newPerson);
      req.flush({ success: true });
    });

    it('getClientById: debe devolver el objeto directo (sin .data)', () => {
      const mockPerson = { personId: 5, name: 'Pedro' };

      service.getClientById(5).subscribe(person => {
        expect(person.name).toBe('Pedro');
      });

      const req = httpMock.expectOne(`${apiUrl}/personas/readbypersonid/5`);
      expect(req.request.method).toBe('GET');
      req.flush(mockPerson); // Devolvemos el objeto directo
    });
  });

  // ==========================================
  // 2. PRUEBAS DE CLIENTES (USUARIOS)
  // ==========================================
  describe('Gestión de Clientes (Usuarios)', () => {
    it('getCustomers: debe enviar parámetros de paginación grandes', () => {
      service.getCustomers().subscribe();

      const req = httpMock.expectOne(req => 
        req.url === `${apiUrl}/clientes/readall` && 
        req.params.get('pageNumber') === '1' &&
        req.params.get('pageSize') === '1000'
      );
      
      expect(req.request.method).toBe('GET');
      req.flush({ data: [] });
    });
  });

  // ==========================================
  // 3. PRUEBAS DE CUENTAS
  // ==========================================
  describe('Gestión de Cuentas', () => {
    it('getAccounts: debe obtener cuentas y mapear la respuesta', () => {
      const mockResponse = { data: [{ accountNumber: 123, initialBalance: 500 }] };

      service.getAccounts().subscribe(accs => {
        expect(accs[0].accountNumber).toBe(123);
      });

      const req = httpMock.expectOne(`${apiUrl}/cuentas/readall`);
      req.flush(mockResponse);
    });

    it('updateAccount: debe enviar petición PUT al endpoint correcto', () => {
      const acc = { accountNumber: 123, initialBalance: 1000 } as Account;

      service.updateAccount(123, acc).subscribe();

      const req = httpMock.expectOne(`${apiUrl}/cuentas/update/123`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(acc);
      req.flush(true);
    });
  });

  // ==========================================
  // 4. PRUEBAS DE MOVIMIENTOS
  // ==========================================
  describe('Gestión de Movimientos', () => {
    it('createTransaction: debe asegurar que los valores sean números', () => {
      const txInput = {
        accountNumber: 12345,
        amount: 100.50,
        transactionType: 'Deposito'
      } as Transaction;

      service.createTransaction(txInput).subscribe();

      const req = httpMock.expectOne(`${apiUrl}/movimientos/create`);
      expect(req.request.method).toBe('POST');
      // Verificamos el payload exacto
      expect(req.request.body).toEqual({
        accountNumber: 12345,
        amount: 100.50,
        transactionType: 'Deposito'
      });
      req.flush(true);
    });

    it('getTransactions: debe enviar paginación', () => {
      service.getTransactions().subscribe();

      const req = httpMock.expectOne(r => r.url === `${apiUrl}/movimientos/readall`);
      expect(req.request.params.get('pageSize')).toBe('1000');
      req.flush({ data: [] });
    });
  });

  // ==========================================
  // 5. PRUEBAS DE REPORTES
  // ==========================================
  describe('Reportes', () => {
    it('getReports: debe enviar todos los query params correctamente', () => {
      const startDate = '2025-01-01';
      const endDate = '2025-01-31';
      const clientId = 5;

      service.getReports(startDate, endDate, clientId).subscribe();

      // Verificamos que la URL y los parámetros sean los esperados
      const req = httpMock.expectOne(req => 
        req.url === `${apiUrl}/reportes/movimientosporfechas` &&
        req.params.get('fechaInicio') === startDate &&
        req.params.get('fechaFin') === endDate &&
        req.params.get('clienteId') === '5'
      );

      expect(req.request.method).toBe('GET');
      
      // Simulamos respuesta con PDF
      req.flush({ reporte: [], archivoPdf: 'base64string' });
    });
  });

});