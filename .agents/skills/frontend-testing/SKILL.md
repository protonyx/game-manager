---
name: frontend-testing
description: Guidelines and instructions for testing the frontend of the Game Manager application. Use this when writing tests for the frontend components, including unit tests, component tests, and end-to-end tests.
---

### Frontend Testing

The frontend uses Jasmine and Karma for testing.

#### Adding New Tests

1. Create a `.spec.ts` file alongside the component or service you want to test
2. Use Jasmine's `describe` and `it` functions to structure your tests
3. Use Angular's TestBed for component testing

Example:
```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyComponent } from './my-component.component';

describe('MyComponent', () => {
  let component: MyComponent;
  let fixture: ComponentFixture<MyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyComponent ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
```
