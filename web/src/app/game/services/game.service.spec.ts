import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { GameService } from './game.service';
import { Game, GameState, JoinGame, NewGame } from '../models/models';
import { PatchOperation } from '../models/patch';

describe('GameService', () => {
  let service: GameService;
  let httpMock: HttpTestingController;

  const mockGame: Game = {
    id: 'game-1',
    name: 'Test Game',
    entryCode: 'ABC123',
    options: { shareOtherPlayerTrackers: false },
    state: GameState.Preparing,
    trackers: [],
    createdDate: '2024-01-01',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        GameService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ]
    });
    service = TestBed.inject(GameService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('createGame', () => {
    it('POSTs to a URL containing "Games"', () => {
      const newGame: NewGame = { name: 'My Game', options: { shareOtherPlayerTrackers: false }, trackers: [] };
      service.createGame(newGame).subscribe();
      const req = httpMock.expectOne(r => r.url.includes('Games') && r.method === 'POST');
      expect(req.request.url).toContain('Games');
      req.flush(mockGame);
    });
  });

  describe('joinGame', () => {
    it('POSTs to a URL containing "Games/Join"', () => {
      const joinGame: JoinGame = { entryCode: 'ABC123', name: 'Player1', observer: false };
      service.joinGame(joinGame).subscribe();
      const req = httpMock.expectOne(r => r.url.includes('Games/Join') && r.method === 'POST');
      expect(req.request.url).toContain('Games/Join');
      req.flush({ gameId: 'game-1', playerId: 'p1', token: 'tok', isHost: false });
    });
  });

  describe('getGame', () => {
    it('GETs a URL containing the gameId', () => {
      const gameId = 'game-42';
      service.getGame(gameId).subscribe();
      const req = httpMock.expectOne(r => r.url.includes(gameId) && r.method === 'GET');
      expect(req.request.url).toContain(gameId);
      req.flush(mockGame);
    });
  });

  describe('endTurn', () => {
    it('POSTs to a URL containing gameId and "EndTurn"', () => {
      const gameId = 'game-42';
      service.endTurn(gameId).subscribe();
      const req = httpMock.expectOne(r =>
        r.url.includes(gameId) && r.url.includes('EndTurn') && r.method === 'POST'
      );
      expect(req.request.url).toContain('EndTurn');
      req.flush(null);
    });
  });

  describe('startGame', () => {
    it('POSTs to a URL containing gameId and "Start"', () => {
      const gameId = 'game-42';
      service.startGame(gameId).subscribe();
      const req = httpMock.expectOne(r =>
        r.url.includes(gameId) && r.url.includes('Start') && r.method === 'POST'
      );
      expect(req.request.url).toContain('Start');
      req.flush(null);
    });
  });

  describe('patchPlayer', () => {
    it('uses application/json-patch+json Content-Type header', () => {
      const ops: PatchOperation[] = [{ op: 'replace', path: '/name', value: 'NewName' }];
      service.patchPlayer('player-1', ops).subscribe();
      const req = httpMock.expectOne(r => r.url.includes('Players') && r.method === 'PATCH');
      expect(req.request.headers.get('Content-Type')).toBe('application/json-patch+json');
      req.flush({ id: 'player-1', order: 1, name: 'NewName', state: 'Active', trackerValues: {} });
    });
  });
});
