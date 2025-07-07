export interface LayoutState {
  title: string;
  sidenavOpen: boolean;
}

export const initialState: LayoutState = {
  title: 'Game Manager',
  sidenavOpen: false,
};
