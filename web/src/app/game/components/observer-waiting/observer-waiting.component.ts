import { Component } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-observer-waiting',
  templateUrl: './observer-waiting.component.html',
  styleUrls: ['./observer-waiting.component.scss'],
  imports: [MatProgressSpinnerModule],
})
export class ObserverWaitingComponent {}
