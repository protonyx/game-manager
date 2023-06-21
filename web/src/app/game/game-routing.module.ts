import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { JoinGamePageComponent } from './pages/join-game-page/join-game-page.component';
import { GameRouteGuard } from './services/game-route-guard.service';
import { GamePageComponent } from './pages/game-page/game-page.component';
import { NewGamePageComponent } from './pages/new-game-page/new-game-page.component';

const routes: Routes = [
  {
    path: '',
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
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GameRoutingModule {}
