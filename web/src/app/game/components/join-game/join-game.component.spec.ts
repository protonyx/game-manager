import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { JoinGameComponent } from './join-game.component';
import { JoinGame } from '../../models/models';
import { PLAYER_COLORS } from '../../models/player-colors';

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

  it('form is invalid when entryCode and playerName are filled but no color selected', () => {
    component.joinGameForm.patchValue({ entryCode: 'ABC123', playerName: 'Alice' });
    expect(component.joinGameForm.valid).toBeFalse();
  });

  it('form is valid when entryCode, playerName and color are all filled', () => {
    component.joinGameForm.patchValue({ entryCode: 'ABC123', playerName: 'Alice', color: '#D32F2F' });
    expect(component.joinGameForm.valid).toBeTrue();
  });

  it('onSubmit() emits joinGame with observer: false and correct values including color', () => {
    component.joinGameForm.patchValue({ entryCode: 'ABC123', playerName: 'Alice', color: '#1976D2' });
    let emitted: JoinGame | undefined;
    component.joinGame.subscribe((v) => (emitted = v));

    component.onSubmit();

    expect(emitted).toEqual({ entryCode: 'ABC123', name: 'Alice', observer: false, color: '#1976D2' });
  });

  it('onJoinAsObserver() emits joinGame with observer: true', () => {
    component.joinGameForm.patchValue({ entryCode: 'XYZ', playerName: 'Bob', color: '#D32F2F' });
    let emitted: JoinGame | undefined;
    component.joinGame.subscribe((v) => (emitted = v));

    component.onJoinAsObserver();

    expect(emitted).toEqual(jasmine.objectContaining({ entryCode: 'XYZ', observer: true }));
  });

  it('emitted values have whitespace trimmed', () => {
    component.joinGameForm.patchValue({ entryCode: '  ABC  ', playerName: '  Alice  ', color: '#D32F2F' });
    let emitted: JoinGame | undefined;
    component.joinGame.subscribe((v) => (emitted = v));

    component.onSubmit();

    expect(emitted?.entryCode).toBe('ABC');
    expect(emitted?.name).toBe('Alice');
  });

  it('renders a color swatch button for each available color', () => {
    const swatches = fixture.nativeElement.querySelectorAll('button.color-swatch');
    expect(swatches.length).toBe(PLAYER_COLORS.length);
  });

  it('selectColor() updates the form color value', () => {
    component.selectColor('#388E3C');

    expect(component.joinGameForm.value.color).toBe('#388E3C');
  });

  it('isColorSelected() returns true for the currently selected color', () => {
    component.joinGameForm.patchValue({ color: '#388E3C' });

    expect(component.isColorSelected('#388E3C')).toBeTrue();
  });

  it('isColorSelected() returns false for a non-selected color', () => {
    component.joinGameForm.patchValue({ color: '#388E3C' });

    expect(component.isColorSelected('#D32F2F')).toBeFalse();
  });

  it('isColorSelected() is case-insensitive', () => {
    component.joinGameForm.patchValue({ color: '#D32F2F' });

    expect(component.isColorSelected('#d32f2f')).toBeTrue();
  });
});
