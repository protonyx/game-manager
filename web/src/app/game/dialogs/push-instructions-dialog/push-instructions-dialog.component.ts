import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface PushInstructionsDialogData {
  title: string;
  message: string;
}

@Component({
  selector: 'app-push-instructions-dialog',
  templateUrl: './push-instructions-dialog.component.html',
  imports: [MatButtonModule, MatDialogModule],
})
export class PushInstructionsDialogComponent {
  readonly data = inject<PushInstructionsDialogData>(MAT_DIALOG_DATA);
}
