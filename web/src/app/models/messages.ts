import {Game, Player} from "./models";

export interface GameStateChangedMessage {
    gameId: string
    game: Game
}

export interface PlayerJoinedMessage {
    gameId: string
    player: Player
}

export interface PlayerStateChangedMessage {
    gameId: string
    player: Player
}

export interface PlayerLeftMessage {
    gameId: string
    playerId: string
}
