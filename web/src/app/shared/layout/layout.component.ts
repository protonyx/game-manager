import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Store} from "@ngrx/store";
import {selectTitle} from "../state/layout.reducer";
import {selectGame} from "../../game/state/game.reducer";
import {map} from "rxjs";
import {MediaMatcher} from "@angular/cdk/layout";
import {GameActions} from "../../game/state/game.actions";
import {Router} from "@angular/router";

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

  @Output()
  public leaveGame: EventEmitter<unknown> = new EventEmitter<unknown>()

  mobileQuery: MediaQueryList;

  constructor(private media: MediaMatcher) {
    this.mobileQuery = this.media.matchMedia('(max-width: 1000px)')
  }

  onLeaveGame(): void {
    this.leaveGame.emit();
  }
}
