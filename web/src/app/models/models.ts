

export interface NewGame {
    name: string
    options: GameOptions
    trackers: Tracker[]
}

export interface JoinGame {
    entryCode: string
    name: string
}

export interface Game {
    id: string
    name: string
    entryCode: string
    players: Player[]
    trackers: Tracker[]
}

export interface GameOptions {
    shareOtherPlayerTrackers: boolean
}

export interface Tracker {
    name: string
    startingValue: number
}

export interface Player {
    order: number
    name: string
    trackerValues: object
}

export interface PlayerCredentials {
    gameId: string
    playerId: string
    token: string
}