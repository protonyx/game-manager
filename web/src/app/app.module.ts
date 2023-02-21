import {NgModule, isDevMode} from '@angular/core';
import {HttpClientModule} from "@angular/common/http";
import {BrowserModule} from '@angular/platform-browser';
import {ReactiveFormsModule} from '@angular/forms';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {StoreModule} from '@ngrx/store';
import {StoreRouterConnectingModule, routerReducer} from '@ngrx/router-store';
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {JoinGameComponent} from './components/join-game/join-game.component';
import {JoinGamePageComponent} from './pages/join-game-page/join-game-page.component';
import {gamesReducer} from "./state/games.reducer";
import { StoreDevtoolsModule } from '@ngrx/store-devtools';

@NgModule({
    declarations: [
        AppComponent,
        JoinGameComponent,
        JoinGamePageComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        StoreModule.forRoot({
            router: routerReducer,
            game: gamesReducer
        }, {}),
        StoreRouterConnectingModule.forRoot(),
        ReactiveFormsModule,
        HttpClientModule,
        MatInputModule,
        MatButtonModule,
        StoreDevtoolsModule.instrument({ maxAge: 25, logOnly: !isDevMode() }),
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule {
}
