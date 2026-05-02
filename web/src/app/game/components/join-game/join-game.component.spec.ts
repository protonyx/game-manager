import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { JoinGameComponent } from './join-game.component';
import { JoinGame } from '../../models/models';

describe('JoinGameComponent', () => {
  let component: JoinGameComponent;
  let fixture: ComponentFixture<JoinGameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JoinGameComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(JoinGameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('form is invalid when empty', () => {
    expect(component.joinGameForm.valid).toBeFalse();
  });

  it('form is valid when entryCode and playerName are filled', () => {
    component.joinGameForm.patchValue({ entryCode: 'ABC123', playerName: 'Alice' });
    expect(component.joinGameForm.valid).toBeTrue();
  });

  it('onSubmit() emits joinGame with observer: false and correct values', () => {
    component.joinGameForm.patchValue({ entryCode: 'ABC123', playerName: 'Alice' });
    let emitted: JoinGame | undefined;
    component.joinGame.subscribe((v) => (emitted = v));

    component.onSubmit();

    expect(emitted).toEqual({ entryCode: 'ABC123', name: 'Alice', observer: false });
  });

  it('onJoinAsObserver() emits joinGame with observer: true', () => {
    component.joinGameForm.patchValue({ entryCode: 'XYZ', playerName: 'Bob' });
    let emitted: JoinGame | undefined;
    component.joinGame.subscribe((v) => (emitted = v));

    component.onJoinAsObserver();

    expect(emitted).toEqual(jasmine.objectContaining({ entryCode: 'XYZ', observer: true }));
  });

  it('emitted values have whitespace trimmed', () => {
    component.joinGameForm.patchValue({ entryCode: '  ABC  ', playerName: '  Alice  ' });
    let emitted: JoinGame | undefined;
    component.joinGame.subscribe((v) => (emitted = v));

    component.onSubmit();

    expect(emitted?.entryCode).toBe('ABC');
    expect(emitted?.name).toBe('Alice');
  });

  it('does not render a color picker', () => {
    const colorPicker = fixture.nativeElement.querySelector('.color-picker');
    expect(colorPicker).toBeNull();
  });
});
