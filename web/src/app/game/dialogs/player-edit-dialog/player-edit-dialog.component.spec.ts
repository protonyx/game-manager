import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { PlayerEditDialogComponent } from './player-edit-dialog.component';
import { PlayerEditFormValue } from '../../components/player-edit/player-edit.component';
import { Tracker } from '../../models/models';

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

  describe('formatOps', () => {
    it('returns only name op when trackers is null', () => {
      const value: PlayerEditFormValue = { name: 'Bob', trackers: {} };
      const ops = component.formatOps(value, null);
      expect(ops).toEqual([{ op: 'replace', path: '/name', value: 'Bob' }]);
    });

    it('returns name op plus tracker ops when trackers are provided', () => {
      const value: PlayerEditFormValue = { name: 'Alice', trackers: { t1: 5 } };
      const trackers: Tracker[] = [{ id: 't1', name: 'Score', startingValue: 0 }];
      const ops = component.formatOps(value, trackers);
      expect(ops.length).toBe(2);
      expect(ops[0]).toEqual({ op: 'replace', path: '/name', value: 'Alice' });
      expect(ops[1]).toEqual({ op: 'replace', path: '/trackerValues/t1', value: 5 });
    });
  });
});
