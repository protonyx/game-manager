import {createActionGroup, props} from "@ngrx/store";
import {Game, JoinGame, PlayerCredentials} from "../models/models";

export const GamesActions = createActionGroup({
    source: 'Games',
    events: {
        'Join Game': props<{ joinGame: JoinGame }>(),
        'Game Updated': props<{ game: Game }>()
    }
});

export const GamesApiActions = createActionGroup({
    source: 'Games API',
    events: {
        'Joined Game': props<{ credentials: PlayerCredentials }>(),
        'Retrieved Game State': props<{ game: Game }>()
    }
});