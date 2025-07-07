import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss'],
    imports: [
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        MatSidenavModule,
        MatListModule,
    ],
})
export class LayoutComponent {
  @Input()
  public title: string | null | undefined = 'Game Manager';

  @Input()
  public sidenavOpen: boolean | null | undefined;

  @Output()
  public closeSidenav: EventEmitter<void> = new EventEmitter<void>();

  @Output()
  public toggleSidenav: EventEmitter<void> = new EventEmitter<void>();

  mobileQuery: MediaQueryList;

  constructor(private media: MediaMatcher) {
    this.mobileQuery = this.media.matchMedia('(max-width: 1000px)');
  }
}
