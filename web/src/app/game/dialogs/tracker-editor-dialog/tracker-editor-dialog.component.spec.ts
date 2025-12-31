import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackerEditorDialogComponent } from './tracker-editor-dialog.component';

describe('TrackerEditorDialogComponent', () => {
  let component: TrackerEditorDialogComponent;
  let fixture: ComponentFixture<TrackerEditorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackerEditorDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrackerEditorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
