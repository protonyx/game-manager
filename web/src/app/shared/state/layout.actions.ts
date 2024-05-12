import { createActionGroup, emptyProps, props } from '@ngrx/store';

export const LayoutActions = createActionGroup({
  source: 'Layout',
  events: {
    'Set Title': props<{ title: string }>(),
    'Set Entry Code': props<{ entryCode: string }>(),
    'Set Header': props<{ title: string; entryCode: string }>(),
    'Reset Layout': emptyProps(),
  },
});
