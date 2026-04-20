import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { MatTable } from '@angular/material/table';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { PlayerReorderDialogComponent } from './player-reorder-dialog.component';
import { Player } from '../../models/models';

describe('PlayerReorderDialogComponent', () => {
  let component: PlayerReorderDialogComponent;
  let fixture: ComponentFixture<PlayerReorderDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerReorderDialogComponent, NoopAnimationsModule],
      providers: [provideMockStore()],
    }).compileComponents();

    fixture = TestBed.createComponent(PlayerReorderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('dropTable moves item and updates order values', () => {
    const players: Player[] = [
      { id: 'p1', name: 'A', order: 1, state: 'Active', trackerValues: {} },
      { id: 'p2', name: 'B', order: 2, state: 'Active', trackerValues: {} },
    ];
    component.newPlayers = players;

    const event = { previousIndex: 0, currentIndex: 1 } as CdkDragDrop<Player[]>;
    const mockTable = { renderRows: jasmine.createSpy('renderRows') } as unknown as MatTable<Player>;

    component.dropTable(event, mockTable);

    expect(component.newPlayers[0].id).toBe('p2');
    expect(component.newPlayers[0].order).toBe(1);
    expect(component.newPlayers[1].id).toBe('p1');
    expect(component.newPlayers[1].order).toBe(2);
    expect(mockTable.renderRows).toHaveBeenCalled();
  });
});
