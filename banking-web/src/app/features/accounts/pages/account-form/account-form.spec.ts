//import { jest } from '@jest/globals';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AccountForm } from './account-form';
import { BankApi } from '../../../../core/services/bank-api';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';

describe('AccountForm Component', () => {
  let component: AccountForm;
  let fixture: ComponentFixture<AccountForm>;
  
  // Spies (Espías) para simular las dependencias
  let bankServiceSpy: any;
  let routerSpy: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    // 1. Configuración de Mocks
    bankServiceSpy = {
      getCustomers: jest.fn().mockReturnValue(of([])), // Retorna observable vacío
      getAccountById: jest.fn().mockReturnValue(of({ 
        accountNumber: 123456, 
        initialBalance: 1000,
        accountType: 'Ahorro',
        customerId: 1,
        status: true 
      })),
      addAccount: jest.fn().mockReturnValue(of(true)),
      updateAccount: jest.fn().mockReturnValue(of(true))
    };

    routerSpy = {
      navigate: jest.fn()
    };

    // Mock dinámico para ActivatedRoute (lo configuraremos en cada test)
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jest.fn()
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [AccountForm, ReactiveFormsModule], // Componente Standalone y Forms
      providers: [
        { provide: BankApi, useValue: bankServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();
  });

  // =================================================
  // ESCENARIO A: MODO CREACIÓN (Ruta sin ID)
  // =================================================
  describe('Modo Creación (Nueva Cuenta)', () => {
    
    beforeEach(() => {
      // Simulamos que NO viene ID en la URL
      activatedRouteMock.snapshot.paramMap.get.mockReturnValue(null);
      fixture = TestBed.createComponent(AccountForm);
      component = fixture.componentInstance;
      fixture.detectChanges(); // Ejecuta ngOnInit
    });

    it('debería inicializar el formulario con un número de cuenta aleatorio', () => {
      const accNumber = component.form.get('accountNumber')?.value;
      expect(accNumber).toBeTruthy();
      expect(accNumber).toBeGreaterThanOrEqual(100000); // Verifica rango
    });

    it('debería tener el campo "accountNumber" deshabilitado por defecto', () => {
      expect(component.form.get('accountNumber')?.disabled).toBeTruthy();
    });

    it('debería llamar a addAccount al guardar', () => {
      // Llenamos el formulario (el accountNumber ya se generó solo)
      component.form.patchValue({
        accountType: 'Corriente',
        initialBalance: 500,
        customerId: 1
      });

      component.save();

      // Verificamos que llamó al servicio de CREAR
      expect(bankServiceSpy.addAccount).toHaveBeenCalled();
      
      // Verificamos que se enviaron los datos (incluso el disabled)
      const dataEnviada = bankServiceSpy.addAccount.mock.calls[0][0];
      expect(dataEnviada.accountNumber).toBeTruthy();
      expect(dataEnviada.initialBalance).toBe(500);

      // Verificamos redirección
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/cuentas']);
    });
  });

  // =================================================
  // ESCENARIO B: MODO EDICIÓN (Ruta con ID)
  // =================================================
  describe('Modo Edición', () => {
    
    beforeEach(() => {
      // Simulamos que SÍ viene ID en la URL (ej. 123456)
      activatedRouteMock.snapshot.paramMap.get.mockReturnValue('123456');
      fixture = TestBed.createComponent(AccountForm);
      component = fixture.componentInstance;
      fixture.detectChanges(); // Ejecuta ngOnInit
    });

    it('debería cargar los datos de la cuenta existente', () => {
      expect(bankServiceSpy.getAccountById).toHaveBeenCalledWith(123456);
      // Verifica que el formulario se llenó con el mock del servicio
      expect(component.form.get('initialBalance')?.value).toBe(1000);
    });

    it('debería bloquear (disable) los campos críticos en edición', () => {
      // Verificamos tus 4 bloqueos específicos
      expect(component.form.get('accountNumber')?.disabled).toBeTruthy();
      expect(component.form.get('customerId')?.disabled).toBeTruthy();
      expect(component.form.get('accountType')?.disabled).toBeTruthy();
      expect(component.form.get('initialBalance')?.disabled).toBeTruthy();
      
      // El estado debe estar habilitado
      expect(component.form.get('status')?.disabled).toBeFalsy();
    });

    it('debería llamar a updateAccount al guardar', () => {
      // Cambiamos el estado (único campo editable)
      component.form.patchValue({ status: false });

      component.save();

      // Verificamos llamada a ACTUALIZAR
      expect(bankServiceSpy.updateAccount).toHaveBeenCalled();
      
      // Verificamos argumentos: ID y Objeto
      const idEnviado = bankServiceSpy.updateAccount.mock.calls[0][0];
      const dataEnviada = bankServiceSpy.updateAccount.mock.calls[0][1];
      
      expect(idEnviado).toBe(123456);
      expect(dataEnviada.status).toBe(false);
      // getRawValue() debió enviar también los campos bloqueados
      expect(dataEnviada.initialBalance).toBe(1000);

      expect(routerSpy.navigate).toHaveBeenCalledWith(['/cuentas']);
    });
  });

  // =================================================
  // ESCENARIO C: VALIDACIONES
  // =================================================
  describe('Validaciones', () => {
    beforeEach(() => {
      activatedRouteMock.snapshot.paramMap.get.mockReturnValue(null);
      fixture = TestBed.createComponent(AccountForm);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it('no debería llamar al servicio si el formulario es inválido', () => {
      // Dejamos el formulario inválido (ej. sin cliente)
      component.form.patchValue({ customerId: '' });
      
      component.save();

      expect(bankServiceSpy.addAccount).not.toHaveBeenCalled();
      expect(bankServiceSpy.updateAccount).not.toHaveBeenCalled();
    });
  });

});