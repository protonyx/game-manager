import {
  createAction,
  createActionGroup,
  emptyProps,
  props,
} from "@ngrx/store";
import {
  Game,
  JoinGame,
  Player,
  PlayerCredentials,
  TrackerValue,
} from "../models/models";
import {
  GameStateChangedMessage,
  PlayerJoinedMessage,
  PlayerLeftMessage,
  PlayerStateChangedMessage,
} from "../models/messages";

export const GameActions = createActionGroup({
  source: "Games",
  events: {
    "Join Game": props<{ joinGame: JoinGame }>(),
    "Leave Game": emptyProps(),
    "Clear Credentials": emptyProps(),
    "Update Tracker": props<{ tracker: TrackerValue }>(),
  },
});

export const GameHubActions = createActionGroup({
  source: "GameHub",
  events: {
    "Hub Connected": emptyProps(),
    "Hub Disconnected": emptyProps(),
    "Game Updated": props<GameStateChangedMessage>(),
    "Player Joined": props<PlayerJoinedMessage>(),
    "Player Updated": props<PlayerStateChangedMessage>(),
    "Player Left": props<PlayerLeftMessage>(),
  },
});

export const GamesApiActions = createActionGroup({
  source: "Games API",
  events: {
    "Created Game": props<{ game: Game }>(),
    "Joined Game": props<{ credentials: PlayerCredentials }>(),
    "Retrieved Game": props<{ game: Game }>(),
    "Retrieved Players": props<{ players: Player[] }>(),
    "Retrieved Current Player": props<{ player: Player }>(),
    "Authentication Error": emptyProps(),
  },
});

export const PlayersApiActions = createActionGroup({
  source: "Players API",
  events: {
    "Player Updated": props<{ player: Player }>(),
    "Player Removed": props<{ playerId: string }>(),
  },
});
