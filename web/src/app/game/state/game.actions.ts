import {
  createAction,
  createActionGroup,
  emptyProps,
  props,
} from '@ngrx/store';
import {
  Game,
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
  PlayerStateChangedMessage,
} from '../models/messages';

export const GameActions = createActionGroup({
  source: 'Games',
  events: {
    'Create Game': props<{ game: NewGame }>(),
    'Join Game': props<{ joinGame: JoinGame }>(),
    'Leave Game': emptyProps(),
    'Clear Credentials': emptyProps(),
    'Load Game': props<{ gameId: string }>(),
    'Load Current Player': props<{ playerId: string }>(),
    'Load Players': props<{ gameId: string }>(),
    'Remove Player': props<{ playerId: string }>(),
    'Update Tracker': props<{ tracker: TrackerValue }>(),
    'Update Player Order': props<{ playerId: string; order: number }>(),
  },
});

export const GameHubActions = createActionGroup({
  source: 'GameHub',
  events: {
    'Hub Connected': emptyProps(),
    'Hub Disconnected': emptyProps(),
    'Game Updated': props<GameStateChangedMessage>(),
    'Player Joined': props<PlayerJoinedMessage>(),
    'Player Updated': props<PlayerStateChangedMessage>(),
    'Player Left': props<PlayerLeftMessage>(),
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
    'Retrieved Players': props<{ players: Player[] }>(),
    'Retrieved Players Error': props<{ error: string }>(),
    'Retrieved Current Player': props<{ player: Player }>(),
    'Retrieved Player Error': props<{ error: string }>(),
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
