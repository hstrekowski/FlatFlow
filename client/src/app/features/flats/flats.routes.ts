import { Routes } from '@angular/router';

export const FLATS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./flats-list.component').then((m) => m.FlatsListComponent),
  },
];
