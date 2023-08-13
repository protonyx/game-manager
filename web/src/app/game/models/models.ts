export interface NewGame {
  name: string;
  options: GameOptions;
  trackers: Tracker[];
}

export interface JoinGame {
  entryCode: string;
  name: string;
}

export interface Game {
  id: string;
  name: string;
  entryCode: string;
  options: GameOptions;
  state: string;
  currentTurnPlayerId?: string;
  lastTurnStartTime?: string;
  trackers: Tracker[];
  createdDate: string;
  startedDate?: string;
  completedDate?: string;
}

export interface GameSummary {
  id: string;
  name: string;
  trackers: Tracker[];
  players: PlayerSummary[];
  createdDate: string;
  startedDate?: string;
  completedDate?: string;
}

export interface PlayerSummary {
  id: string;
  order: number;
  name: string;
  turns: PlayerTurn[];
  trackerHistory: TrackerHistory[];
}

export interface GameOptions {
  shareOtherPlayerTrackers: boolean;
}

export interface Tracker {
  id: string;
  name: string;
  startingValue: number;
}

export interface TrackerValue {
  trackerId: string;
  value: number;
}

export interface TrackerHistory {
  trackerId: string;
  changedTime: string;
  newValue: number;
  secondsSinceGameStart?: number;
}

export interface PlayerTrackerHistory extends TrackerHistory {
  playerId: string;
}

export interface TrackerSummary extends Tracker {
  trackerHistory: PlayerTrackerHistory[];
}

export interface Player {
  id: string;
  order: number;
  name: string;
  trackerValues: any;
}

export interface PlayerTurn {
  startTime: string;
  endTime: string;
  durationSeconds: number;
}

export interface PlayerCredentials {
  gameId: string;
  playerId: string;
  token: string;
  isAdmin: boolean;
}
