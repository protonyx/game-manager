import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {JoinGamePageComponent} from "./pages/join-game-page/join-game-page.component";
import {GamePageComponent} from "./pages/game-page/game-page.component";

const routes: Routes = [
    {path: '', component: JoinGamePageComponent},
    {path: 'game', component: GamePageComponent}
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
