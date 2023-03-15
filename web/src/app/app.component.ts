import { Component } from '@angular/core';
import {selectTitle} from "./shared/state/layout.reducer";
import {selectGame} from "./game/state/game.reducer";
import {map} from "rxjs";
import {Store} from "@ngrx/store";
import {Router} from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'GameManager';

  title$ = this.store.select(selectTitle)

  game$ = this.store.select(selectGame)

  entryCode$ = this.game$.pipe(
      map(g => g?.entryCode)
  )

  constructor(private store: Store,
              private router: Router) {
    this.game$.subscribe(game => {
      if (game) {
        router.navigate(['game'])
      } else {
        router.navigate(['game', 'join'])
      }
    })
  }
}
