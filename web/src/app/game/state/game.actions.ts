import {
  createAction,
  createActionGroup,
  emptyProps,
  props,
} from '@ngrx/store';
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

export const GameActions = createActionGroup({
  source: 'Games',
  events: {
    'Create Game': props<{ game: NewGame }>(),
    'Join Game': props<{ joinGame: JoinGame }>(),
    'Leave Game': emptyProps(),
    'Clear Credentials': emptyProps(),
    'Load Game': props<{ gameId: string }>(),
    'Load Game Summary': props<{ gameId: string }>(),
    'Load Players': props<{ gameId: string }>(),
    'Remove Player': props<{ playerId: string }>(),
    'Load Player': props<{ playerId: string }>(),
    'Update Tracker': props<{ playerId: string; tracker: TrackerValue }>(),
    'Update Player Order': props<{ gameId: string; players: Player[] }>(),
    'End Turn': props<{ gameId: string }>(),
    'Start Game': props<{ gameId: string }>(),
    'End Game': props<{ gameId: string }>(),
  },
});

export const GameHubActions = createActionGroup({
  source: 'GameHub',
  events: {
    'Hub Connected': emptyProps(),
    'Hub Disconnected': emptyProps(),
    'Game Updated': props<GameStateChangedMessage>(),
    'Player Joined': props<PlayerJoinedMessage>(),
    'Player Updated': props<PlayerUpdatedMessage>(),
    'Player Left': props<PlayerLeftMessage>(),
    'Credentials Updated': props<{ credentials: PlayerCredentials }>(),
  },
});

export const GamesApiActions = createActionGroup({
  source: 'Games API',
  events: {
    'Created Game': props<{ game: Game }>(),
    'Created Game Error': props<{ error: string }>(),
    'Joined Game': props<{
      credentials: PlayerCredentials;
      entryCode: string;
    }>(),
    'Joined Game Error': props<{ error: string }>(),
    'Left Game': emptyProps(),
    'Retrieved Game': props<{ game: Game }>(),
    'Retrieved Game Error': props<{ error: string }>(),
    'Retrieved Game Summary': props<{ summary: GameSummary }>(),
    'Retrieved Game Summary Error': props<{ error: string }>(),
    'Retrieved Players': props<{ players: Player[] }>(),
    'Retrieved Players Error': props<{ error: string }>(),
    'Retrieved Player': props<{ player: Player }>(),
    'Retrieved Player Error': props<{ error: string }>(),
    'End Turn Succeeded': emptyProps(),
    'End Turn Error': props<{ error: string }>(),
    'Authentication Error': emptyProps(),
  },
});

export const PlayersApiActions = createActionGroup({
  source: 'Players API',
  events: {
    'Player Updated': props<{ player: Player }>(),
    'Player Updated Error': props<{ error: string }>(),
    'Player Removed': props<{ playerId: string }>(),
    'Player Removed Error': props<{ error: string }>(),
  },
});
