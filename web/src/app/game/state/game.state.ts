import { Game, Player, PlayerCredentials } from '../models/models';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';

export interface GameState {
  hubConnected: boolean;
  credentials: PlayerCredentials | null;
  game: Game | null;
  players: EntityState<Player>;
}

export function selectPlayerId(a: Player): string {
  return a.id;
}

export function sortPlayersByName(a: Player, b: Player): number {
  return a.name.localeCompare(b.name);
}

export function sortPlayersByOrder(a: Player, b: Player): number {
  return a.order - b.order;
}

export const playerAdapter: EntityAdapter<Player> = createEntityAdapter<Player>(
  {
    selectId: selectPlayerId,
    sortComparer: sortPlayersByOrder,
  }
);

export const initialState: GameState = {
  hubConnected: false,
  credentials: null,
  game: null,
  players: playerAdapter.getInitialState(),
};
