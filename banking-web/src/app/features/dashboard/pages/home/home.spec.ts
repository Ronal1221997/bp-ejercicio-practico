import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { BankApi } from '../../../../core/services/bank-api';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('Home', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;

  // 1. Creamos un Mock del servicio (simulación)
  const mockBankService = {
    // Simulamos que getClients devuelve 2 clientes
    getClients: jest.fn().mockReturnValue(of([
      { id: 1, name: 'Cliente 1' }, 
      { id: 2, name: 'Cliente 2' }
    ])),
    // Simulamos que getAccounts devuelve 2 cuentas con saldo
    getAccounts: jest.fn().mockReturnValue(of([
      { id: 1, initialBalance: 1000 },
      { id: 2, initialBalance: 500 }
    ]))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [
        provideRouter([]), // Para los routerLink del HTML
        // 2. Reemplazamos el servicio real por el Mock
        { provide: BankApi, useValue: mockBankService } 
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
    
    // 3. detectChanges dispara ngOnInit y consume los observables del mock
    fixture.detectChanges(); 
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería calcular los totales correctamente al iniciar', () => {
    // Como el mock devolvió 2 clientes y cuentas con saldo 1000+500
    expect(component.totalClients).toBe(2);
    expect(component.totalAccounts).toBe(2);
    expect(component.totalMoney).toBe(1500);
  });
});