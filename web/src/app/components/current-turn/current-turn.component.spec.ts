import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentTurnComponent } from './current-turn.component';

describe('CurrentTurnComponent', () => {
  let component: CurrentTurnComponent;
  let fixture: ComponentFixture<CurrentTurnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CurrentTurnComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CurrentTurnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
