import { TestBed } from '@angular/core/testing';

import { PlayerService } from './player.service';
import { Player } from '../models/models';

describe('PlayerService', () => {
  let service: PlayerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PlayerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getPlayerColor', () => {
    it('returns default gray when player is undefined', () => {
      expect(service.getPlayerColor(undefined)).toBe('#cccccc');
    });

    it('returns default gray when called with no argument', () => {
      expect(service.getPlayerColor()).toBe('#cccccc');
    });

    it('returns an HSL color string for a valid player', () => {
      const player: Player = { id: 'abc', order: 1, name: 'Alice', color: '', state: 'Active', trackerValues: {} };
      const color = service.getPlayerColor(player);
      expect(color).toMatch(/^hsl\(-?\d+, 70%, 60%\)$/);
    });

    it('returns the same color for the same player (deterministic)', () => {
      const player: Player = { id: 'player-123', order: 1, name: 'Bob', color: '', state: 'Active', trackerValues: {} };
      const color1 = service.getPlayerColor(player);
      const color2 = service.getPlayerColor(player);
      expect(color1).toBe(color2);
    });

    it('returns different colors for players with different IDs', () => {
      const player1: Player = { id: 'aaaa', order: 1, name: 'Alice', color: '', state: 'Active', trackerValues: {} };
      const player2: Player = { id: 'zzzz', order: 2, name: 'Bob', color: '', state: 'Active', trackerValues: {} };
      expect(service.getPlayerColor(player1)).not.toBe(service.getPlayerColor(player2));
    });

    it('returns player.color directly when the color property is set', () => {
      const player: Player = { id: 'p1', order: 1, name: 'Alice', color: '#D32F2F', state: 'Active', trackerValues: {} };
      expect(service.getPlayerColor(player)).toBe('#D32F2F');
    });

    it('falls back to hash-based color when player.color is empty', () => {
      const player: Player = { id: 'abc', order: 1, name: 'Alice', color: '', state: 'Active', trackerValues: {} };
      const color = service.getPlayerColor(player);
      expect(color).toMatch(/^hsl\(-?\d+, 70%, 60%\)$/);
    });
  });
});
