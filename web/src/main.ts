import { AppComponent } from "./app/app.component";
import { GameEffects } from "./app/game/state/game.effects";
import { EffectsModule } from "@ngrx/effects";
import { isDevMode, importProvidersFrom } from "@angular/core";
import { StoreDevtoolsModule } from "@ngrx/store-devtools";
import { ReactiveFormsModule } from "@angular/forms";
import { localStorageSync } from "ngrx-store-localstorage";
import { reducer as layoutReducer } from "./app/shared/state/layout.reducer";
import {
  reducer as gameReducer,
  name as gameName,
} from "./app/game/state/game.reducer";
import { routerReducer, StoreRouterConnectingModule } from "@ngrx/router-store";
import { LayoutState } from "./app/shared/state/layout.state";
import { GameState } from "./app/game/state/game.state";
import {
  StoreModule,
  ActionReducerMap,
  MetaReducer,
  ActionReducer,
} from "@ngrx/store";
import { provideAnimations } from "@angular/platform-browser/animations";
import { AppRoutingModule } from "./app/app-routing.module";
import { BrowserModule, bootstrapApplication } from "@angular/platform-browser";
import { AuthInterceptorService } from "./app/game/services/auth-interceptor.service";
import {
  HTTP_INTERCEPTORS,
  withInterceptorsFromDi,
  provideHttpClient,
} from "@angular/common/http";

const reducers: ActionReducerMap<appState> = {
  router: routerReducer,
  game: gameReducer,
  layout: layoutReducer,
};

interface appState {
  router: any;
  game: GameState;
  layout: LayoutState;
}

export function localStorageSyncReducer(
  reducer: ActionReducer<any>
): ActionReducer<any> {
  return localStorageSync({
    keys: [gameName],
    rehydrate: true,
  })(reducer);
}

const metaReducers: Array<MetaReducer<any, any>> = [localStorageSyncReducer];

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      BrowserModule,
      AppRoutingModule,
      StoreModule.forRoot(reducers, { metaReducers }),
      StoreRouterConnectingModule.forRoot(),
      ReactiveFormsModule,
      StoreDevtoolsModule.instrument({
        maxAge: 25,
        logOnly: !isDevMode(),
      }),
      EffectsModule.forRoot([GameEffects])
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptorService,
      multi: true,
    },
    provideAnimations(),
    provideHttpClient(withInterceptorsFromDi()),
  ],
}).catch((err) => console.error(err));
