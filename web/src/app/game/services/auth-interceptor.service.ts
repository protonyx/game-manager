import { Injectable } from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";
import {selectCredentials} from "../state/game.reducer";
import {PlayerCredentials} from "../models/models";

@Injectable()
export class AuthInterceptorService implements HttpInterceptor {

  credentials: PlayerCredentials | null | undefined;

  constructor(private store: Store) {
    this.store.select(selectCredentials)
      .subscribe(c => this.credentials = c);
}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authToken = this.credentials?.token;

    if (authToken) {
      const authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${authToken}`
        }
      });

      return next.handle(authReq);
    }

    return next.handle(req);
  }
}
