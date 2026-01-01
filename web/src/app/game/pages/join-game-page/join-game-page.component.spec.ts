import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideMockActions } from '@ngrx/effects/testing';
import { of } from 'rxjs';

import { JoinGamePageComponent } from './join-game-page.component';

describe('JoinGamePageComponent', () => {
  let component: JoinGamePageComponent;
  let fixture: ComponentFixture<JoinGamePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JoinGamePageComponent, NoopAnimationsModule],
      providers: [
        provideMockStore(),
        provideMockActions(() => of()),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(JoinGamePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
