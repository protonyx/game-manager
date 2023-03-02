import {createActionGroup, props} from "@ngrx/store";
import {Game, JoinGame, Player, PlayerCredentials} from "../models/models";
import {
    GameStateChangedMessage,
    PlayerJoinedMessage,
    PlayerLeftMessage,
    PlayerStateChangedMessage
} from "../models/messages";

export const GameActions = createActionGroup({
    source: 'Games',
    events: {
        'Join Game': props<{ joinGame: JoinGame }>(),
    }
});

export const GameHubActions = createActionGroup({
    source: 'GameHub',
    events: {
        'Game Updated': props<GameStateChangedMessage>(),
        'Player Joined': props<PlayerJoinedMessage>(),
        'Player Updated': props<PlayerStateChangedMessage>(),
        'Player Left': props<PlayerLeftMessage>(),
    }
});

export const GamesApiActions = createActionGroup({
    source: 'Games API',
    events: {
        'Created Game': props<{ game: Game }>(),
        'Joined Game': props<{ credentials: PlayerCredentials }>(),
        'Retrieved Game': props<{ game: Game }>(),
        'Retrieved Players': props<{ players: Player[] }>(),
        'Retrieved Current Player': props<{ player: Player }>()
    }
});