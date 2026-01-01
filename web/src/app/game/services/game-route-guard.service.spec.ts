import { TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';

import { GameRouteGuard } from './game-route-guard.service';

describe('GameRouteGuardService', () => {
  let service: GameRouteGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        GameRouteGuard,
        provideMockStore(),
      ]
    });
    service = TestBed.inject(GameRouteGuard);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
