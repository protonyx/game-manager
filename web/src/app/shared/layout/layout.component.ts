import { Component } from '@angular/core';
import {Store} from "@ngrx/store";
import {selectTitle} from "../state/layout.reducer";
import {selectGame} from "../../game/state/game.reducer";
import {map} from "rxjs";

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {

  title$ = this.store.select(selectTitle)

  entryCode$ = this.store.select(selectGame).pipe(
      map(g => g?.entryCode)
  )

  constructor(private store: Store) {}
}
