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
import {
  HTTP_INTERCEPTORS,
  withInterceptorsFromDi,
  provideHttpClient,
} from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { provideHighcharts } from 'highcharts-angular';

const SESSION_ID_KEY = 'game_manager_session_id';
export const SESSION_STORAGE_PREFIX = 'game_manager_session_';

export function generateUUID(): string {
  if (
    typeof crypto !== 'undefined' &&
    typeof crypto.randomUUID === 'function'
  ) {
    return crypto.randomUUID();
  }

  if (
    typeof crypto !== 'undefined' &&
    typeof crypto.getRandomValues === 'function'
  ) {
    try {
      const bytes = new Uint8Array(16);
      crypto.getRandomValues(bytes);
      bytes[6] = (bytes[6] & 0x0f) | 0x40;
      bytes[8] = (bytes[8] & 0x3f) | 0x80;
      const hex: string[] = [];
      for (let i = 0; i < 16; i++) {
        hex.push(bytes[i].toString(16).padStart(2, '0'));
      }
      return `${hex.slice(0, 4).join('')}-${hex.slice(4, 6).join('')}-${hex.slice(6, 8).join('')}-${hex.slice(8, 10).join('')}-${hex.slice(10, 16).join('')}`;
    } catch {
      // Fall through to Math.random fallback
    }
  }

  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

export function getSessionId(): string {
  let sessionId: string | null = null;
  try {
    sessionId = sessionStorage.getItem(SESSION_ID_KEY);
  } catch {
    // sessionStorage may be restricted
  }

  if (!sessionId) {
    sessionId = generateUUID();
    try {
      sessionStorage.setItem(SESSION_ID_KEY, sessionId);
    } catch {
      // sessionStorage may be restricted
    }
  }
  return sessionId;
}

export function clearSessionId(): void {
  try {
    sessionStorage.removeItem(SESSION_ID_KEY);
  } catch {
    // sessionStorage may be restricted
  }
}

export function localStorageSyncReducer(
  reducer: ActionReducer<unknown>,
): ActionReducer<unknown> {
  return localStorageSync({
    keys: [{ [gameFeatureKey]: ['credentials'] }],
    rehydrate: true,
    storageKeySerializer: (key) =>
      `${SESSION_STORAGE_PREFIX}${getSessionId()}_${key}`,
  })(reducer);
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const metaReducers: MetaReducer<any, any>[] = [localStorageSyncReducer];

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
    provideHighcharts(),
  ],
}).catch((err) => console.error(err));
