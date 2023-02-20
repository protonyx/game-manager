import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {JoinGamePageComponent} from "./pages/join-game-page/join-game-page.component";

const routes: Routes = [
  { path: '', component: JoinGamePageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
