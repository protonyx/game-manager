import { AppComponent } from './app/app.component';
import * as gameEffects from './app/game/state/game.effects';
import { provideEffects } from '@ngrx/effects';
import { isDevMode } from '@angular/core';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { localStorageSync } from 'ngrx-store-localstorage';
import {
  name as layoutFeatureKey,
  reducer as layoutReducer,
} from './app/shared/state/layout.reducer';
import {
  gameFeatureKey,
  reducer as gameReducer,
} from './app/game/state/game.reducer';
import { provideRouterStore, routerReducer } from '@ngrx/router-store';
import { MetaReducer, ActionReducer, provideStore } from '@ngrx/store';
import { provideAnimations } from '@angular/platform-browser/animations';
import { bootstrapApplication } from '@angular/platform-browser';
import { AuthInterceptorService } from './app/game/services/auth-interceptor.service';
import { HTTP_INTERCEPTORS, withInterceptorsFromDi, provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { provideHighcharts } from 'highcharts-angular';

export function localStorageSyncReducer(
  reducer: ActionReducer<unknown>,
): ActionReducer<unknown> {
  return localStorageSync({
    keys: [{ [gameFeatureKey]: ['credentials'] }],
    rehydrate: true,
  })(reducer);
}

const metaReducers: Array<MetaReducer<any, any>> = [localStorageSyncReducer];

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideStore(
      {
        router: routerReducer,
        [layoutFeatureKey]: layoutReducer,
        [gameFeatureKey]: gameReducer,
      },
      { metaReducers },
    ),
    provideRouterStore(),
    provideEffects(gameEffects),
    provideStoreDevtools({
      name: 'Game Manager',
      maxAge: 25,
      logOnly: !isDevMode(),
    }),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptorService,
      multi: true,
    },
    provideAnimations(),
    provideHttpClient(withInterceptorsFromDi()),
    provideHighcharts()
  ],
}).catch((err) => console.error(err));
