import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/game',
    pathMatch: 'full',
  },
  {
    path: 'game',
    loadChildren: () => import('./game/routes'),
  },
];
