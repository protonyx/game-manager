import { createActionGroup, props } from '@ngrx/store';

export const LayoutActions = createActionGroup({
  source: 'Layout',
  events: {
    'Set Title': props<{ title: string }>(),
  },
});
