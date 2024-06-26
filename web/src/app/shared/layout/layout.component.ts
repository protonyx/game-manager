import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import { RouterLinkActive, RouterLink, RouterOutlet } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NgIf } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
  standalone: true,
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    NgIf,
    MatSidenavModule,
    MatListModule,
    RouterLinkActive,
    RouterLink,
    RouterOutlet,
  ],
})
export class LayoutComponent {
  @Input()
  public title: string | null | undefined = 'Game Manager';

  @Input()
  public entryCode: string | null | undefined;

  @Input()
  public sidenavOpen: boolean | null | undefined;

  @Output()
  public leaveGame: EventEmitter<unknown> = new EventEmitter<unknown>();

  @Output()
  public closeSidenav: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public toggleSidenav: EventEmitter<void> = new EventEmitter<void>();

  mobileQuery: MediaQueryList;

  constructor(private media: MediaMatcher) {
    this.mobileQuery = this.media.matchMedia('(max-width: 1000px)');
  }

  onLeaveGame(): void {
    this.leaveGame.emit();
    this.closeSidenav.emit();
  }
}
