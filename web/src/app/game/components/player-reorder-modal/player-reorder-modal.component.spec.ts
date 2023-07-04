import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayerReorderModalComponent } from './player-reorder-modal.component';

describe('PlayerReorderModalComponent', () => {
  let component: PlayerReorderModalComponent;
  let fixture: ComponentFixture<PlayerReorderModalComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [PlayerReorderModalComponent],
    });
    fixture = TestBed.createComponent(PlayerReorderModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
