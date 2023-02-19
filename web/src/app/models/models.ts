

export class NewGame {
    name: string
    options: GameOptions = new GameOptions()
    trackers: Tracker[] = []
}

export class JoinGame {
    entryCode: string
    name: string
}

export class Game {
    id: string
    name: string
    entryCode: string
    players: Player[]
    trackers: Tracker[]
}

export class GameOptions {
    shareOtherPlayerTrackers: boolean = true
}

export class Tracker {
    name: string
    startingValue: number = 0
}

export class Player {
    order: number
    name: string
    trackerValues: object
}

export class PlayerCredentials {
    playerId: string
    token: string
}