

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
    options: GameOptions
    currentTurnPlayerId: string
    lastTurnStartTime: string
    trackers: Tracker[]
}

export interface GameOptions {
    shareOtherPlayerTrackers: boolean
}

export interface Tracker {
    id: string
    name: string
    startingValue: number
}

export interface TrackerValue {
    trackerId: string,
    value: number
}

export interface Player {
    id: string
    order: number
    name: string
    trackerValues: any
}

export interface PlayerCredentials {
    gameId: string
    playerId: string
    token: string
    isAdmin: boolean
}