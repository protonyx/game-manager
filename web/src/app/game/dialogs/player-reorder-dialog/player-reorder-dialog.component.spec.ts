import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayerReorderDialogComponent } from './player-reorder-dialog.component';

describe('PlayerReorderDialogComponent', () => {
  let component: PlayerReorderDialogComponent;
  let fixture: ComponentFixture<PlayerReorderDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlayerReorderDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PlayerReorderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
