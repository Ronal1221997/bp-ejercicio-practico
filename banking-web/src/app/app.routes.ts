import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'dashboard',
    // Carga perezosa (Lazy Load) del componente principal
    loadComponent: () => import('./features/dashboard/pages/home/home')
      .then(m => m.Home)
  },
  {
    path: 'clientes',
    // Ruta para la pantalla que vamos a crear
    loadComponent: () => import('./features/clients/pages/client-list/client-list')
      .then(m => m.ClientList)
  },
  {
    // NUEVA RUTA: Agregamos /nuevo
    path: 'clientes/nuevo',
    loadComponent: () => import('./features/clients/pages/client-form/client-form').then(m => m.ClientForm)
  },
  {
    // Los dos puntos :id indican que es un valor dinámico
    path: 'clientes/editar/:id',
    // Reutilizamos el MISMO componente del formulario
    loadComponent: () => import('./features/clients/pages/client-form/client-form').then(m => m.ClientForm)
  },
  {
    path: 'clientes/asignar/:id',
    loadComponent: () => import('./features/clients/pages/customer-create/customer-create').then(m => m.CustomerCreate)
  },
  {
    path: 'cuentas',
    loadComponent: () => import('./features/accounts/pages/account-list/account-list').then(m => m.AccountList)
  },
  {
    path: 'cuentas/nueva',
    loadComponent: () => import('./features/accounts/pages/account-form/account-form').then(m => m.AccountForm)
  },
  {
    // Ruta para EDITAR (con el parámetro :id)
    path: 'cuentas/editar/:id',
    loadComponent: () => import('./features/accounts/pages/account-form/account-form').then(m => m.AccountForm)
  },
  {
    path: 'movimientos',
    loadComponent: () => import('./features/transactions/pages/transaction-list/transaction-list').then(m => m.TransactionList)
  },
  {
    path: 'reportes',
    loadComponent: () => import('./features/reports/pages/report-list/report-list').then(m => m.ReportList)
  },
  {
    path: 'movimientos',
    loadComponent: () => import('./features/transactions/pages/transaction-list/transaction-list').then(m => m.TransactionList)
  },
  {
    path: 'movimientos/nuevo',
    loadComponent: () => import('./features/transactions/pages/transaction-form/transaction-form').then(m => m.TransactionForm)
  },
  {
    // Redireccionar la ruta vacía directamente al dashboard
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    // Cualquier ruta desconocida te lleva al dashboard
    path: '**',
    redirectTo: 'dashboard'
  }
];