import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';

import { PlayerReorderDialogComponent } from './player-reorder-dialog.component';

describe('PlayerReorderDialogComponent', () => {
  let component: PlayerReorderDialogComponent;
  let fixture: ComponentFixture<PlayerReorderDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerReorderDialogComponent],
      providers: [provideMockStore()],
    }).compileComponents();

    fixture = TestBed.createComponent(PlayerReorderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
