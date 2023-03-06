import {Game, Player, PlayerCredentials} from "../models/models";

export interface GameState {
    credentials: PlayerCredentials | null;
    currentPlayer: Player | null;
    game: Game | null;
    players: Player[];
}

export const initialState: GameState = {
    credentials: null,
    currentPlayer: null,
    game: null,
    players: []
};
