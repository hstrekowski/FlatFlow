import { Routes } from '@angular/router';

export const FLAT_DETAIL_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./flat-detail-layout/flat-detail-layout.component').then(
        (m) => m.FlatDetailLayoutComponent,
      ),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./dashboard/flat-dashboard.component').then(
            (m) => m.FlatDashboardComponent,
          ),
      },
      {
        path: 'chores',
        loadComponent: () =>
          import('./chores/chores-list/chores-list.component').then(
            (m) => m.ChoresListComponent,
          ),
      },
      {
        path: 'payments',
        loadComponent: () =>
          import('./payments/payments-list/payments-list.component').then(
            (m) => m.PaymentsListComponent,
          ),
      },
      {
        path: 'notes',
        loadComponent: () =>
          import('./notes/notes-list/notes-list.component').then(
            (m) => m.NotesListComponent,
          ),
      },
      {
        path: 'tenants',
        loadComponent: () =>
          import('./tenants/tenants-list/tenants-list.component').then(
            (m) => m.TenantsListComponent,
          ),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./settings/flat-settings.component').then(
            (m) => m.FlatSettingsComponent,
          ),
      },
    ],
  },
];
