import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { GameActions } from '../../state/game.actions';
import { selectSummary } from '../../state/game.reducer';
import { ActivatedRoute } from '@angular/router';
import { filter, map, Subject } from 'rxjs';

@Component({
  selector: 'app-game-summary-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-summary-page.component.html',
  styleUrls: ['./game-summary-page.component.scss'],
})
export class GameSummaryPageComponent implements OnInit, OnDestroy {
  gameSummary$ = this.store.select(selectSummary);

  unsubscribe$: Subject<boolean> = new Subject<boolean>();

  constructor(private store: Store, private route: ActivatedRoute) {}

  ngOnInit() {
    this.route.paramMap
      .pipe(
        filter((params) => params.has('id')),
        map((params) => params.get('id'))
      )
      .subscribe((id) => {
        this.store.dispatch(GameActions.loadGameSummary({ gameId: id! }));
      });
  }

  ngOnDestroy() {
    this.unsubscribe$.next(true);
    this.unsubscribe$.unsubscribe();
  }
}
