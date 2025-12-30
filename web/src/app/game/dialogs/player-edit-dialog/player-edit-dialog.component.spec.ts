import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { PlayerEditDialogComponent } from './player-edit-dialog.component';

describe('PlayerEditDialogComponent', () => {
  let component: PlayerEditDialogComponent;
  let fixture: ComponentFixture<PlayerEditDialogComponent>;

  const initialState = {
    game: {
      players: {
        ids: ['1'],
        entities: {
          '1': { id: '1', name: 'Player 1', order: 1 }
        }
      },
      trackers: []
    }
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerEditDialogComponent, NoopAnimationsModule],
      providers: [
        provideMockStore({ initialState }),
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: { playerId: '1' } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PlayerEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
