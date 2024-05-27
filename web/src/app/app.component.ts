import { Component } from '@angular/core';
import { selectEntryCode, selectTitle, selectSidenavOpen } from './shared/state/layout.reducer';
import { Store } from '@ngrx/store';
import { Router } from '@angular/router';
import { GameActions } from './game/state/game.actions';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './shared/layout/layout.component';
import { LayoutActions } from './shared/state/layout.actions';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: true,
  imports: [CommonModule, LayoutComponent],
})
export class AppComponent {
  title$ = this.store.select(selectTitle);

  entryCode$ = this.store.select(selectEntryCode);

  sidenavOpen$ = this.store.select(selectSidenavOpen);

  constructor(
    private store: Store,
    private router: Router,
  ) {}

  onLeaveGame(): void {
    this.store.dispatch(GameActions.leaveGame());
    this.router.navigate(['game', 'join']);
  }

  toggleSidenav(): void {
    this.store.dispatch(LayoutActions.toggleSidenav())
  }

  closeSidenav(): void {
    this.store.dispatch(LayoutActions.closeSidenav());
  }
}
