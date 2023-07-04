import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Store } from '@ngrx/store';
import { selectCredentials } from '../state/game.reducer';
import { PlayerCredentials } from '../models/models';
import { GamesApiActions } from '../state/game.actions';

@Injectable()
export class AuthInterceptorService implements HttpInterceptor {
  credentials: PlayerCredentials | null | undefined;

  constructor(private store: Store) {
    this.store
      .select(selectCredentials)
      .subscribe((c) => (this.credentials = c));
  }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const authToken = this.credentials?.token;

    if (authToken) {
      const authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${authToken}`,
        },
      });

      return next.handle(authReq).pipe(
        tap({
          error: (err) => {
            if (err.status === 401) {
              this.store.dispatch(GamesApiActions.authenticationError());
            }
          },
        })
      );
    }

    return next.handle(req);
  }
}
