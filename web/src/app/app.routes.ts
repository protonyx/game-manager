import { Routes } from '@angular/router';
import { provideState } from '@ngrx/store';
import {
  gameFeatureKey,
  reducer as gameReducer,
} from './game/state/game.reducer';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/game',
    pathMatch: 'full',
  },
  {
    path: 'game',
    loadChildren: () => import('./game/routes'),
    providers: [provideState(gameFeatureKey, gameReducer)],
  },
];
