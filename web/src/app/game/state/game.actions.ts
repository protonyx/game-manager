import { createActionGroup, emptyProps, props } from '@ngrx/store';
import {
  Game,
  GameSummary,
  JoinGame,
  NewGame,
  Player,
  PlayerCredentials,
  TrackerValue,
} from '../models/models';
import {
  GameStateChangedMessage,
  PlayerJoinedMessage,
  PlayerLeftMessage,
  PlayerUpdatedMessage,
} from '../models/messages';
import { PatchOperation } from '../models/patch';

export const GameActions = createActionGroup({
  source: 'Games Page',
  events: {
    createGame: props<{ game: NewGame }>(),
    joinGame: props<{ joinGame: JoinGame }>(),
    leaveGame: emptyProps(),
    clearCredentials: emptyProps(),
    loadGame: props<{ gameId: string }>(),
    loadGameSummary: props<{ gameId: string }>(),
    loadPlayers: props<{ gameId: string }>(),
    editPlayer: props<{ playerId: string }>(),
    removePlayer: props<{ playerId: string }>(),
    loadPlayer: props<{ playerId: string }>(),
    updateTracker: props<{ playerId: string; tracker: TrackerValue }>(),
    reorderPlayers: emptyProps(),
    endTurn: props<{ gameId: string }>(),
    startGame: props<{ gameId: string }>(),
    endGame: props<{ gameId: string }>(),
  },
});

export const GameHubActions = createActionGroup({
  source: 'GameHub',
  events: {
    hubReconnect: emptyProps(),
    hubConnected: emptyProps(),
    hubDisconnected: emptyProps(),
    gameUpdated: props<GameStateChangedMessage>(),
    gameEnded: props<{ gameId: string }>(),
    playerJoined: props<PlayerJoinedMessage>(),
    playerUpdated: props<PlayerUpdatedMessage>(),
    playerLeft: props<PlayerLeftMessage>(),
    credentialsUpdated: props<{ credentials: PlayerCredentials }>(),
  },
});

export const GamesApiActions = createActionGroup({
  source: 'Games API',
  events: {
    createdGame: props<{ game: Game }>(),
    createdGameError: props<{ error: string }>(),
    joinedGame: props<{
      credentials: PlayerCredentials;
      entryCode: string;
    }>(),
    joinedGameError: props<{ error: string }>(),
    leftGame: emptyProps(),
    retrievedGame: props<{ game: Game }>(),
    retrievedGameError: props<{ error: string }>(),
    retrievedGameSummary: props<{ summary: GameSummary }>(),
    retrievedGameSummaryError: props<{ error: string }>(),
    retrievedPlayers: props<{ players: Player[] }>(),
    retrievedPlayersError: props<{ error: string }>(),
    retrievedPlayer: props<{ player: Player }>(),
    retrievedPlayerError: props<{ error: string }>(),
    patchPlayer: props<{ playerId: string; ops: PatchOperation[] }>(),
    patchedPlayer: props<{ player: Player }>(),
    patchedPlayerError: props<{ error: string }>(),
    endTurnSucceeded: emptyProps(),
    endTurnError: props<{ error: string }>(),
    authenticationError: emptyProps(),
  },
});

export const PlayerReorderDialogActions = createActionGroup({
  source: 'Player Reorder Dialog',
  events: {
    closed: emptyProps(),
    updatePlayerOrder: props<{ players: Player[] }>(),
  },
});

export const PlayersApiActions = createActionGroup({
  source: 'Players API',
  events: {
    playerUpdated: props<{ player: Player }>(),
    playerUpdatedError: props<{ error: string }>(),
    playerRemoved: props<{ playerId: string }>(),
    playerRemovedError: props<{ error: string }>(),
  },
});
