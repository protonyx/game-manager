import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { first, mergeMap, Observable, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import { selectCredentials } from '../state/game.selectors';
import { GamesApiActions } from '../state/game.actions';

@Injectable()
export class AuthInterceptorService implements HttpInterceptor {
  credentials$ = this.store.select(selectCredentials);

  constructor(private store: Store) {}

  intercept(
    req: HttpRequest<unknown>,
    next: HttpHandler,
  ): Observable<HttpEvent<unknown>> {
    return this.credentials$.pipe(
      first(),
      mergeMap((cred) => {
        const authReq = cred
          ? req.clone({
              setHeaders: { Authorization: `Bearer ${cred.token}` },
            })
          : req;
        return next.handle(authReq);
      }),
      tap({
        error: (err) => {
          if (err.status === 401) {
            this.store.dispatch(GamesApiActions.authenticationError());
          }
        },
      }),
    );
  }
}
