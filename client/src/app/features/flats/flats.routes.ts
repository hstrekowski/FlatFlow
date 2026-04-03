import { Routes } from '@angular/router';

export const FLATS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./flats-list/flats-list.component').then((m) => m.FlatsListComponent),
  },
  {
    path: ':id',
    loadChildren: () =>
      import('../flat-detail/flat-detail.routes').then((m) => m.FLAT_DETAIL_ROUTES),
  },
];
