import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideMockStore, MockStore } from '@ngrx/store/testing';

import { GameRouteGuard } from './game-route-guard.service';
import { selectCredentials } from '../state/game.selectors';

describe('GameRouteGuardService', () => {
  let service: GameRouteGuard;
  let store: MockStore;
  let routerSpy: { navigate: jasmine.Spy };

  beforeEach(() => {
    routerSpy = { navigate: jasmine.createSpy('navigate') };

    TestBed.configureTestingModule({
      providers: [
        GameRouteGuard,
        provideMockStore(),
        { provide: Router, useValue: routerSpy },
      ]
    });
    service = TestBed.inject(GameRouteGuard);
    store = TestBed.inject(MockStore);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('canActivate', () => {
    it('returns true when credentials are present', (done) => {
      store.overrideSelector(selectCredentials, {
        gameId: 'g1', playerId: 'p1', token: 'tok', isHost: false
      });
      store.refreshState();

      service.canActivate().subscribe(result => {
        expect(result).toBeTrue();
        expect(routerSpy.navigate).not.toHaveBeenCalled();
        done();
      });
    });

    it('returns false and navigates to game/join when no credentials', (done) => {
      store.overrideSelector(selectCredentials, null);
      store.refreshState();

      service.canActivate().subscribe(result => {
        expect(result).toBeFalse();
        expect(routerSpy.navigate).toHaveBeenCalledWith(['game', 'join']);
        done();
      });
    });
  });
});
