import {createFeature, createReducer, on} from "@ngrx/store";
import {initialState} from "./layout.state";
import {LayoutActions} from "./layout.actions";

export const layoutFeature = createFeature({
    name: 'layout',
    reducer: createReducer(
        initialState,
        on(LayoutActions.setTitle, (state, {title}) => {
            return {...state, title: title}
        })
    )
});

export const {
    name,
    reducer,
    selectLayoutState,
    selectTitle
} = layoutFeature;
