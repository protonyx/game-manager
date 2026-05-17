import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SimpleChange } from '@angular/core';

import { TrackerEditorComponent } from './tracker-editor.component';

describe('TrackerEditorComponent', () => {
  let component: TrackerEditorComponent;
  let fixture: ComponentFixture<TrackerEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackerEditorComponent, NoopAnimationsModule],
      providers: [provideMockStore()],
    }).compileComponents();

    fixture = TestBed.createComponent(TrackerEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('@Input() playerColor defaults to "#888888"', () => {
    expect(component.playerColor).toBe('#888888');
  });

  it('onPlusClick() sets keypadActive=true, isAdding=true, keypadValue=""', () => {
    component.onPlusClick();

    expect(component.keypadActive).toBeTrue();
    expect(component.isAdding).toBeTrue();
    expect(component.keypadValue).toBe('');
  });

  it('onMinusClick() sets keypadActive=true, isAdding=false', () => {
    component.onMinusClick();

    expect(component.keypadActive).toBeTrue();
    expect(component.isAdding).toBeFalse();
  });

  it('onKeypadClick() appends digit to keypadValue', () => {
    component.keypadValue = '1';
    component.onKeypadClick('5');

    expect(component.keypadValue).toBe('15');
  });

  it('onKeypadClick("backspace") removes last character', () => {
    component.keypadValue = '123';
    component.onKeypadClick('backspace');

    expect(component.keypadValue).toBe('12');
  });

  it('onCancel() sets keypadActive=false', () => {
    component.keypadActive = true;
    component.onCancel();

    expect(component.keypadActive).toBeFalse();
  });

  it('onSave() with isAdding=true emits trackerChange with +delta', () => {
    component.trackerValueControl.setValue(0);
    component.keypadValue = '5';
    component.isAdding = true;
    let emitted: number | undefined;
    component.trackerChange.subscribe((v) => (emitted = v));

    component.onSave();

    expect(emitted).toBe(5);
    expect(component.keypadActive).toBeFalse();
  });

  it('onSave() with isAdding=false emits trackerChange with -delta', () => {
    component.trackerValueControl.setValue(0);
    component.keypadValue = '3';
    component.isAdding = false;
    let emitted: number | undefined;
    component.trackerChange.subscribe((v) => (emitted = v));

    component.onSave();

    expect(emitted).toBe(-3);
  });

  it('onSave() with empty keypadValue does not emit', () => {
    component.keypadValue = '';
    let emitCount = 0;
    component.trackerChange.subscribe(() => emitCount++);

    component.onSave();

    expect(emitCount).toBe(0);
    expect(component.keypadActive).toBeFalse();
  });

  it('ngOnChanges sets trackerValueControl when trackerValue changes', () => {
    component.trackerValue = 42;
    component.ngOnChanges({ trackerValue: new SimpleChange(null, 42, false) });

    expect(component.trackerValueControl.value).toBe(42);
  });

  it('updateTracker(10) adds delta to current value and emits result', () => {
    component.trackerValueControl.setValue(0);
    let emitted: number | undefined;
    component.trackerChange.subscribe((v) => (emitted = v));

    component.updateTracker(10);

    expect(emitted).toBe(10);
    expect(component.trackerValueControl.value).toBe(10);
  });

  it('setTrackerValue(50) sets control value and emits 50', () => {
    let emitted: number | undefined;
    component.trackerChange.subscribe((v) => (emitted = v));

    component.setTrackerValue(50);

    expect(emitted).toBe(50);
    expect(component.trackerValueControl.value).toBe(50);
  });
});
