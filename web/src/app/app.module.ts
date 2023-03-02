import {NgModule, isDevMode} from '@angular/core';
import {HttpClientModule} from "@angular/common/http";
import {BrowserModule} from '@angular/platform-browser';
import {ReactiveFormsModule} from '@angular/forms';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ActionReducer, ActionReducerMap, MetaReducer, StoreModule} from '@ngrx/store';
import {StoreRouterConnectingModule, routerReducer} from '@ngrx/router-store';
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {JoinGameComponent} from './components/join-game/join-game.component';
import {JoinGamePageComponent} from './pages/join-game-page/join-game-page.component';
import {name as gameName, reducer as gameReducer} from "./state/game.reducer";
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { GamePageComponent } from './pages/game-page/game-page.component';
import {GameState} from "./state/game.state";
import {localStorageSync} from "ngrx-store-localstorage";
import { CurrentTurnComponent } from './components/current-turn/current-turn.component';
import {MatCardModule} from "@angular/material/card";
import { NewGameComponent } from './components/new-game/new-game.component';
import { NewGamePageComponent } from './pages/new-game-page/new-game-page.component';
import {MatCheckboxModule} from "@angular/material/checkbox";
import {MatIconModule} from "@angular/material/icon";

interface appState {
    router: any,
    game: GameState
}


const reducers: ActionReducerMap<appState> = {
    router: routerReducer,
    game: gameReducer
};

export function localStorageSyncReducer(reducer: ActionReducer<any>): ActionReducer<any> {
    return localStorageSync({
        keys: [gameName],
        rehydrate: true
    })(reducer);
}
const metaReducers: Array<MetaReducer<any, any>> = [localStorageSyncReducer];

@NgModule({
    declarations: [
        AppComponent,
        JoinGameComponent,
        JoinGamePageComponent,
        GamePageComponent,
        CurrentTurnComponent,
        NewGameComponent,
        NewGamePageComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        StoreModule.forRoot(
            reducers,
            {metaReducers}
        ),
        StoreRouterConnectingModule.forRoot(),
        ReactiveFormsModule,
        HttpClientModule,
        MatInputModule,
        MatButtonModule,
        StoreDevtoolsModule.instrument({maxAge: 25, logOnly: !isDevMode()}),
        MatCardModule,
        MatCheckboxModule,
        MatIconModule,
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule {
}
