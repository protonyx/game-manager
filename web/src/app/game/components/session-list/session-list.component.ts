import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { StoredSession } from '../../services/session.service';

@Component({
    selector: 'app-session-list',
    templateUrl: './session-list.component.html',
    styleUrls: ['./session-list.component.scss'],
    imports: [
        CommonModule,
        MatListModule,
        MatButtonModule,
        MatIconModule,
        MatCardModule,
    ]
})
export class SessionListComponent {
  @Input()
  sessions: StoredSession[] = [];

  @Output()
  rejoin = new EventEmitter<StoredSession>();

  onRejoin(session: StoredSession): void {
    this.rejoin.emit(session);
  }
}
