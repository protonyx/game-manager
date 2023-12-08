import { Game, Player } from './models';

export interface GameStateChangedMessage {
  gameId: string;
  game: Game;
}

export interface PlayerJoinedMessage {
  gameId: string;
  player: Player;
}

export interface PlayerUpdatedMessage {
  gameId: string;
  playerId: string;
  player: Player;
}

export interface PlayerLeftMessage {
  gameId: string;
  playerId: string;
}
