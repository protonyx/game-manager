import { Game, Player, PlayerCredentials } from '../models/models';

export interface GameState {
  hubConnected: boolean;
  credentials: PlayerCredentials | null;
  currentPlayer: Player | null;
  game: Game | null;
  players: Player[];
}

export const initialState: GameState = {
  hubConnected: false,
  credentials: null,
  currentPlayer: null,
  game: null,
  players: [],
};
