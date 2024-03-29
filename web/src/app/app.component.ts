import { Component } from '@angular/core';
import { selectEntryCode, selectTitle } from './shared/state/layout.reducer';
import { Store } from '@ngrx/store';
import { ActivatedRoute, Router } from '@angular/router';
import { GameActions } from './game/state/game.actions';
import { AsyncPipe, CommonModule } from '@angular/common';
import { LayoutComponent } from './shared/layout/layout.component';

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

  constructor(private store: Store, private router: Router) {}

  onLeaveGame(): void {
    this.store.dispatch(GameActions.leaveGame());
    this.router.navigate(['game', 'join']);
  }
}
