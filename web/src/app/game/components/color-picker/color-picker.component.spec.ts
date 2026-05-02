import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { By } from '@angular/platform-browser';
import { ColorPickerComponent } from './color-picker.component';
import { PLAYER_COLORS } from '../../models/player-colors';

describe('ColorPickerComponent', () => {
  let component: ColorPickerComponent;
  let fixture: ComponentFixture<ColorPickerComponent>;

  const RED = PLAYER_COLORS[0].hex;   // '#D32F2F'
  const BLUE = PLAYER_COLORS[1].hex;  // '#1976D2'
  const GREEN = PLAYER_COLORS[2].hex; // '#388E3C'

  function swatches(): HTMLButtonElement[] {
    return fixture.debugElement
      .queryAll(By.css('.color-swatch'))
      .map((de) => de.nativeElement as HTMLButtonElement);
  }

  function swatchFor(hex: string): HTMLButtonElement | undefined {
    return swatches().find(
      (btn) =>
        btn.style.backgroundColor.toLowerCase() ===
          hexToRgb(hex) ||
        btn.getAttribute('aria-label') ===
          PLAYER_COLORS.find(
            (c) => c.hex.toLowerCase() === hex.toLowerCase()
          )?.name
    );
  }

  /** Convert '#RRGGBB' to 'rgb(r, g, b)' for style comparison. */
  function hexToRgb(hex: string): string {
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    return `rgb(${r}, ${g}, ${b})`;
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ColorPickerComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(ColorPickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // ── 1. Creation ──────────────────────────────────────────────────────────────

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // ── 2. Renders all PLAYER_COLORS ─────────────────────────────────────────────

  it('renders one swatch button for every PLAYER_COLOR', () => {
    expect(swatches().length).toBe(PLAYER_COLORS.length);
  });

  it('sets aria-label on each swatch to its color name', () => {
    const labels = swatches().map((btn) => btn.getAttribute('aria-label'));
    PLAYER_COLORS.forEach((c) => expect(labels).toContain(c.name));
  });

  // ── 3. Label input ───────────────────────────────────────────────────────────

  it('renders default label "Color"', () => {
    const label: HTMLElement = fixture.nativeElement.querySelector('.color-field-label');
    expect(label.textContent?.trim()).toBe('Color');
  });

  it('renders updated label when input changes', () => {
    component.label = 'Pick a color';
    fixture.detectChanges();
    const label: HTMLElement = fixture.nativeElement.querySelector('.color-field-label');
    expect(label.textContent?.trim()).toBe('Pick a color');
  });

  // ── 4. selectedColor — .selected class ───────────────────────────────────────

  it('adds .selected class to the matching swatch', () => {
    component.selectedColor = RED;
    fixture.detectChanges();
    const selected = fixture.debugElement.queryAll(By.css('.color-swatch.selected'));
    expect(selected.length).toBe(1);
    expect(selected[0].nativeElement.getAttribute('aria-label')).toBe(
      PLAYER_COLORS.find((c) => c.hex === RED)?.name
    );
  });

  it('does not add .selected class to non-matching swatches', () => {
    component.selectedColor = RED;
    fixture.detectChanges();
    const notSelected = fixture.debugElement.queryAll(By.css('.color-swatch:not(.selected)'));
    expect(notSelected.length).toBe(PLAYER_COLORS.length - 1);
  });

  // ── 5. takenColors — .taken class and disabled ────────────────────────────────

  it('adds .taken class and disabled attribute to taken swatches', () => {
    component.takenColors = [RED, BLUE];
    fixture.detectChanges();
    const taken = fixture.debugElement.queryAll(By.css('.color-swatch.taken'));
    expect(taken.length).toBe(2);
    taken.forEach((de) =>
      expect((de.nativeElement as HTMLButtonElement).disabled).toBeTrue()
    );
  });

  it('does not add .taken class to non-taken swatches', () => {
    component.takenColors = [RED];
    fixture.detectChanges();
    const notTaken = fixture.debugElement.queryAll(By.css('.color-swatch:not(.taken)'));
    expect(notTaken.length).toBe(PLAYER_COLORS.length - 1);
  });

  // ── 6. Click non-taken swatch emits hex ──────────────────────────────────────

  it('emits colorSelected with the correct hex when a non-taken swatch is clicked', () => {
    const emitted: string[] = [];
    component.colorSelected.subscribe((hex: string) => emitted.push(hex));
    component.takenColors = [];
    fixture.detectChanges();

    const redSwatch = fixture.debugElement
      .queryAll(By.css('.color-swatch'))
      .find(
        (de) =>
          (de.nativeElement as HTMLButtonElement).getAttribute('aria-label') ===
          PLAYER_COLORS[0].name
      );
    redSwatch!.nativeElement.click();

    expect(emitted).toEqual([RED]);
  });

  // ── 7. Click taken swatch does NOT emit ──────────────────────────────────────

  it('does NOT emit colorSelected when a taken swatch is clicked', () => {
    const emitted: string[] = [];
    component.colorSelected.subscribe((hex: string) => emitted.push(hex));
    component.takenColors = [RED];
    fixture.detectChanges();

    // Call select() directly because the button is disabled and won't fire click in DOM
    component.select(RED);

    expect(emitted).toEqual([]);
  });

  // ── 8. Case-insensitive matching ─────────────────────────────────────────────

  it('isSelected() is case-insensitive', () => {
    component.selectedColor = RED.toUpperCase();
    expect(component.isSelected(RED.toLowerCase())).toBeTrue();
    expect(component.isSelected(RED.toUpperCase())).toBeTrue();
  });

  it('isTaken() is case-insensitive', () => {
    component.takenColors = [RED.toUpperCase()];
    expect(component.isTaken(RED.toLowerCase())).toBeTrue();
    expect(component.isTaken(RED.toUpperCase())).toBeTrue();
  });

  it('adds .selected class when selectedColor uses different casing', () => {
    component.selectedColor = RED.toUpperCase();
    fixture.detectChanges();
    // Component stores PLAYER_COLORS with lower-case hex; class comparison is case-insensitive
    const selected = fixture.debugElement.queryAll(By.css('.color-swatch.selected'));
    expect(selected.length).toBe(1);
  });

  it('adds .taken class when takenColors entry uses different casing', () => {
    component.takenColors = [RED.toUpperCase()];
    fixture.detectChanges();
    const taken = fixture.debugElement.queryAll(By.css('.color-swatch.taken'));
    expect(taken.length).toBe(1);
  });

  // ── 9. Icon rendering ────────────────────────────────────────────────────────

  it('shows check-icon inside the selected swatch', () => {
    component.selectedColor = GREEN;
    fixture.detectChanges();
    const selectedSwatch = fixture.debugElement.query(By.css('.color-swatch.selected'));
    expect(selectedSwatch.query(By.css('.check-icon'))).toBeTruthy();
    expect(selectedSwatch.query(By.css('.taken-icon'))).toBeNull();
  });

  it('shows taken-icon inside a taken swatch', () => {
    component.takenColors = [GREEN];
    fixture.detectChanges();
    const takenSwatch = fixture.debugElement.query(By.css('.color-swatch.taken'));
    expect(takenSwatch.query(By.css('.taken-icon'))).toBeTruthy();
    expect(takenSwatch.query(By.css('.check-icon'))).toBeNull();
  });

  // ── 10. Empty selectedColor ───────────────────────────────────────────────────

  it('has no .selected swatches when selectedColor is empty', () => {
    component.selectedColor = '';
    fixture.detectChanges();
    const selected = fixture.debugElement.queryAll(By.css('.color-swatch.selected'));
    expect(selected.length).toBe(0);
  });

  // ── 11. Empty takenColors ─────────────────────────────────────────────────────

  it('has no disabled swatches when takenColors is empty', () => {
    component.takenColors = [];
    fixture.detectChanges();
    const disabled = swatches().filter((btn) => btn.disabled);
    expect(disabled.length).toBe(0);
  });

  it('has no .taken swatches when takenColors is empty', () => {
    component.takenColors = [];
    fixture.detectChanges();
    const taken = fixture.debugElement.queryAll(By.css('.color-swatch.taken'));
    expect(taken.length).toBe(0);
  });

  // ── select() helper ───────────────────────────────────────────────────────────

  it('select() emits when color is not taken', () => {
    spyOn(component.colorSelected, 'emit');
    component.takenColors = [];
    component.select(BLUE);
    expect(component.colorSelected.emit).toHaveBeenCalledOnceWith(BLUE);
  });

  it('select() does not emit when color is taken', () => {
    spyOn(component.colorSelected, 'emit');
    component.takenColors = [BLUE];
    component.select(BLUE);
    expect(component.colorSelected.emit).not.toHaveBeenCalled();
  });
});
