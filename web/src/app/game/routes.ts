import { Route } from '@angular/router';
import { JoinGamePageComponent } from './pages/join-game-page/join-game-page.component';
import { GameRouteGuard } from './services/game-route-guard.service';
import { GamePageComponent } from './pages/game-page/game-page.component';
import { NewGamePageComponent } from './pages/new-game-page/new-game-page.component';
import { GameSummaryPageComponent } from './pages/game-summary-page/game-summary-page.component';

export default [
  {
    path: '',
    title: 'GamePage',
    canActivate: [GameRouteGuard],
    component: GamePageComponent,
  },
  {
    path: 'join',
    component: JoinGamePageComponent,
  },
  {
    path: 'new',
    component: NewGamePageComponent,
  },
  {
    path: 'summary/:id',
    component: GameSummaryPageComponent,
  },
] as Route[];
