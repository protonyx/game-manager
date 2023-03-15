import {Component, Input} from '@angular/core';
import {Store} from "@ngrx/store";
import {selectTitle} from "../state/layout.reducer";
import {selectGame} from "../../game/state/game.reducer";
import {map} from "rxjs";
import {MediaMatcher} from "@angular/cdk/layout";

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {

  @Input()
  public title: string | null = "Game Manager";

  @Input()
  public entryCode: string | null | undefined;

  mobileQuery: MediaQueryList;

  constructor(private store: Store,
              private media: MediaMatcher) {
    this.mobileQuery = this.media.matchMedia('(max-width: 1000px)')
  }
}
