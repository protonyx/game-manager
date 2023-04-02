import {NgModule, isDevMode} from '@angular/core';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {BrowserModule} from '@angular/platform-browser';
import {ReactiveFormsModule} from '@angular/forms';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ActionReducer, ActionReducerMap, MetaReducer, StoreModule} from '@ngrx/store';
import {StoreRouterConnectingModule, routerReducer} from '@ngrx/router-store';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {name as gameName, reducer as gameReducer} from "./game/state/game.reducer";
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import {GameState} from "./game/state/game.state";
import {localStorageSync} from "ngrx-store-localstorage";
import {SharedModule} from "./shared/shared.module";
import {LayoutState} from "./shared/state/layout.state";
import {reducer as layoutReducer} from "./shared/state/layout.reducer";
import {AuthInterceptorService} from "./game/services/auth-interceptor.service";
import { EffectsModule } from '@ngrx/effects';
import {GameEffects} from "./game/state/game.effects";

interface appState {
    router: any,
    game: GameState,
    layout: LayoutState
}


const reducers: ActionReducerMap<appState> = {
    router: routerReducer,
    game: gameReducer,
    layout: layoutReducer
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
        StoreDevtoolsModule.instrument({maxAge: 25, logOnly: !isDevMode()}),
        SharedModule,
        EffectsModule.forRoot([GameEffects]),
    ],
    providers: [
        {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptorService, multi: true}
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
