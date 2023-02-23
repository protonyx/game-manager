import {Game, Player, PlayerCredentials} from "../models/models";

export interface GameState {
    credentials: PlayerCredentials | null;
    game: Game | null;
    players: Player[];
}