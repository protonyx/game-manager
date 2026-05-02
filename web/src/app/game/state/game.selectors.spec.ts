import { selectTakenColors } from './game.selectors';
import { Player } from '../models/models';

describe('game.selectors', () => {
  const makePlayers = (overrides: Partial<Player>[] = []): Player[] =>
    overrides.map((o, i) => ({
      id: `p${i + 1}`,
      name: `Player ${i + 1}`,
      color: '#D32F2F',
      order: i + 1,
      state: 'Active',
      trackerValues: {},
      ...o,
    }));

  describe('selectTakenColors', () => {
    it('returns colors of all players', () => {
      const players = makePlayers([
        { id: 'p1', color: '#D32F2F' },
        { id: 'p2', color: '#1976D2' },
      ]);

      const result = selectTakenColors('').projector(players);

      expect(result).toEqual(['#D32F2F', '#1976D2']);
    });

    it('excludes the color of the player matching excludePlayerId', () => {
      const players = makePlayers([
        { id: 'p1', color: '#D32F2F' },
        { id: 'p2', color: '#1976D2' },
        { id: 'p3', color: '#388E3C' },
      ]);

      const result = selectTakenColors('p2').projector(players);

      expect(result).toContain('#D32F2F');
      expect(result).toContain('#388E3C');
      expect(result).not.toContain('#1976D2');
    });

    it('returns empty array when there are no players', () => {
      const result = selectTakenColors('p1').projector([]);

      expect(result).toEqual([]);
    });

    it('excludes players with empty/falsy color values', () => {
      const players = makePlayers([
        { id: 'p1', color: '#D32F2F' },
        { id: 'p2', color: '' },
      ]);

      const result = selectTakenColors('').projector(players);

      expect(result).toEqual(['#D32F2F']);
    });

    it('returns empty array when all players are excluded by id', () => {
      const players = makePlayers([{ id: 'p1', color: '#D32F2F' }]);

      const result = selectTakenColors('p1').projector(players);

      expect(result).toEqual([]);
    });
  });
});
