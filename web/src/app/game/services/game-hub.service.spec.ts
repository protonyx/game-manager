import { TestBed } from '@angular/core/testing';
import { provideMockStore } from '@ngrx/store/testing';

import { GameHubService } from './game-hub.service';

describe('GameHubService', () => {
  let service: GameHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        GameHubService,
        provideMockStore(),
      ]
    });
    service = TestBed.inject(GameHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
