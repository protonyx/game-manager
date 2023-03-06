import { createSelector, createFeatureSelector } from '@ngrx/store';
import {GameState} from "./game.state";


// export const selectGameFeature = createFeatureSelector<GameState>('game');
//
// export const selectGame = createSelector(
//     selectGameFeature,
//     (game) => {
//         return game.game;
//     }
// );
//
// export const selectPlayers = createSelector(
//     selectGameFeature,
//     (game) => {
//         return game.players;
//     }
// )
//
// export const selectCredentials = createSelector(
//     selectGameFeature,
//     (game) => {
//         return game.credentials;
//     }
// );
