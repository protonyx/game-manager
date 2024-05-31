import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { selectCredentials } from '../state/game.selectors';

@Injectable({
  providedIn: 'root',
})
export class GameRouteGuard {
  credentials$ = this.store.select(selectCredentials);

  constructor(
    private store: Store,
    private router: Router,
  ) {}

  canActivate(): Observable<boolean> {
    return this.credentials$.pipe(
      map((ev) => {
        if (ev) {
          return true;
        } else {
          this.router.navigate(['game', 'join']);
          return false;
        }
      }),
    );
  }
}
