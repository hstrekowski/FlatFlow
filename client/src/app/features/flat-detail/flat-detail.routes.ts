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
    ],
  },
];
