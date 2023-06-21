import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackerEditorComponent } from './tracker-editor.component';

describe('TrackerEditorComponent', () => {
  let component: TrackerEditorComponent;
  let fixture: ComponentFixture<TrackerEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackerEditorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TrackerEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
