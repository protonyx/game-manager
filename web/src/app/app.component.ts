import { Component, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import {
  selectTitle,
  selectSidenavOpen,
} from './shared/state/layout.reducer';
import {
  selectGame
} from './game/state/game.selectors'
import { Store } from '@ngrx/store';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { GameActions } from './game/state/game.actions';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './shared/layout/layout.component';
import { LayoutActions } from './shared/state/layout.actions';
import { MatButtonModule, MatIconButton } from '@angular/material/button';
import { MatListItem, MatNavList } from '@angular/material/list';
import { MatIcon } from '@angular/material/icon';
import { map } from 'rxjs';
import { LetDirective } from '@ngrx/component';
import { InstallPromptService } from './shared/services/install-prompt.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [
    CommonModule,
    LayoutComponent,
    MatIcon,
    MatIconButton,
    MatButtonModule,
    MatListItem,
    MatNavList,
    RouterLink,
    RouterOutlet,
    RouterLinkActive,
    LetDirective
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AppComponent {
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly installPrompt = inject(InstallPromptService);

  title$ = this.store.select(selectTitle);

  sidenavOpen$ = this.store.select(selectSidenavOpen);

  activeGame$ = this.store.select(selectGame);

  isActiveGame$ = this.activeGame$.pipe(map((game) => game !== null));

  isGameInProgress$ = this.activeGame$.pipe(map((game) => game?.state === 'InProgress'));

  canInstall$ = this.installPrompt.available$;

  async installApp(): Promise<void> {
    try {
      await this.installPrompt.promptInstall();
    } catch (error) {
      console.error('Could not start app installation.', error);
    }
  }

  onLeaveGame(): void {
    this.closeSidenav();
    this.store.dispatch(GameActions.leaveGame());
    this.router.navigate(['game', 'join']);
  }

  toggleSidenav(): void {
    this.store.dispatch(LayoutActions.toggleSidenav());
  }

  closeSidenav(): void {
    this.store.dispatch(LayoutActions.closeSidenav());
  }
}
